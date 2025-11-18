using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Linq;
using System.Text;
using System.Text.Json;
using CustomerInsights.Base.Models.Responses;
using CustomerInsights.NlpRuntime;
using SessionOptions = Microsoft.ML.OnnxRuntime.SessionOptions;

namespace CustomerInsights.NlpService.Runtime;

public sealed class ZeroShotAspectNliOnnx : IDisposable
{
    private readonly InferenceSession _session;
    private readonly WordPieceTokenizer _tok;
    private readonly int _maxLen;
    private readonly string _hypoTpl;
    private readonly string _inputIdsName;
    private readonly string? _attentionMaskName;
    private readonly string? _tokenTypeIdsName;
    private readonly bool _hasAttentionMask;
    private readonly bool _hasTokenTypeIds;
    private readonly float _aspectThreshold;
    private readonly int _maxReportedAspects;

    // Für BERT-MNLI: Logit-Indices i.d.R. [contradiction=0, neutral=1, entailment=2]
    private const int ENTAIL_INDEX = 2;

    public ZeroShotAspectNliOnnx()
    {
        JsonElement cfgRoot = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Config", "models.json"))).RootElement;

        JsonElement config = cfgRoot.GetProperty("AspectNli");
        string onnxPath = Path.Combine(AppContext.BaseDirectory, config.GetProperty("Path").GetString()!);
        string vocabPath = Path.Combine(AppContext.BaseDirectory, config.GetProperty("Vocab").GetString()!);

        _session = new InferenceSession(onnxPath, new SessionOptions());
        _tok = new WordPieceTokenizer(vocabPath, lowerCase: true);
        _maxLen = int.Parse(config.GetProperty("MaxSeqLen").GetRawText());
        _hypoTpl = config.GetProperty("HypothesisTemplate").GetString() ?? string.Empty;

        _aspectThreshold = config.TryGetProperty("MinScore", out JsonElement minScoreElement)
            ? minScoreElement.GetSingle()
            : 0.50f; // Default: 0.50

        _maxReportedAspects = config.TryGetProperty("TopK", out JsonElement topKElement)
            ? Math.Max(0, topKElement.GetInt32())
            : 5; // 0 = keine Begrenzung

        string[] inputNames = _session.InputMetadata.Keys.ToArray();
        _inputIdsName = inputNames.First(n => n.Contains("input_ids", StringComparison.Ordinal));
        _attentionMaskName = inputNames.FirstOrDefault(n => n.Contains("attention_mask", StringComparison.Ordinal));
        _tokenTypeIdsName = inputNames.FirstOrDefault(n => n.Contains("token_type_ids", StringComparison.Ordinal));

        _hasAttentionMask = _attentionMaskName is not null;
        _hasTokenTypeIds = _tokenTypeIdsName is not null;

    }

    public ScoreResult[] ScoreAspects(string text, string[] aspects)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("text must not be null or whitespace.", nameof(text));
        }
        if (aspects is null || aspects.Length == 0)
        {
            throw new ArgumentException("aspects must not be null or empty.", nameof(aspects));
        }

        string[] hypotheses = new string[aspects.Length];
        for (int i = 0; i < aspects.Length; i++)
        {
            hypotheses[i] = _hypoTpl.Replace("{aspect}", aspects[i], StringComparison.Ordinal);
        }

        List<int[]> inputIdsList = new List<int[]>(aspects.Length);
        List<int[]?> tokenTypeIdsList = new List<int[]?>(aspects.Length);
        List<int[]?> attentionMaskList = new List<int[]?>(aspects.Length);

        for (int i = 0; i < aspects.Length; i++)
        {
            (int[] ids, int[] tt, int[] att) = _tok.EncodePair(text, hypotheses[i], _maxLen);
            inputIdsList.Add(ids);
            tokenTypeIdsList.Add(tt);
            attentionMaskList.Add(att);
        }

        int batchMaxLen = inputIdsList.Max(static ids => ids.Length);
        if (batchMaxLen > _maxLen)
        {
            batchMaxLen = _maxLen;
        }

        int totalElements = aspects.Length * batchMaxLen;
        long[] idsFlat = new long[totalElements];
        long[]? ttFlat = _hasTokenTypeIds ? new long[totalElements] : null;
        long[]? maskFlat = _hasAttentionMask ? new long[totalElements] : null;

        for (int row = 0; row < aspects.Length; row++)
        {
            int[] ids = inputIdsList[row];
            int[]? tt = tokenTypeIdsList[row];
            int[]? mask = attentionMaskList[row];

            int copyLen = Math.Min(ids.Length, batchMaxLen);
            int offset = row * batchMaxLen;

            for (int col = 0; col < copyLen; col++)
            {
                idsFlat[offset + col] = ids[col];

                if (ttFlat is not null && tt is not null)
                {
                    ttFlat[offset + col] = tt[col];
                }
                if (maskFlat is not null && mask is not null)
                {
                    maskFlat[offset + col] = mask[col];
                }
            }
        }

        DenseTensor<long> inputIdsTensor = new DenseTensor<long>(
            new Memory<long>(idsFlat),
            new[] { aspects.Length, batchMaxLen }
        );

        List<NamedOnnxValue> modelInputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(_inputIdsName, inputIdsTensor)
        };

        if (_hasAttentionMask && maskFlat is not null && _attentionMaskName is not null)
        {
            DenseTensor<long> attentionMaskTensor = new DenseTensor<long>(
                new Memory<long>(maskFlat),
                new[] { aspects.Length, batchMaxLen }
            );
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_attentionMaskName, attentionMaskTensor));
        }

        if (_hasTokenTypeIds && ttFlat is not null && _tokenTypeIdsName is not null)
        {
            DenseTensor<long> tokenTypeIdsTensor = new DenseTensor<long>(
                new Memory<long>(ttFlat),
                new[] { aspects.Length, batchMaxLen }
            );
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_tokenTypeIdsName, tokenTypeIdsTensor));
        }

        IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs = _session.Run(modelInputs);
        try
        {
            float[] logitsFlat = outputs.First().AsEnumerable<float>().ToArray();
            const int numClasses = 3;
            if (logitsFlat.Length != aspects.Length * numClasses)
            {
                throw new InvalidDataException("Unexpected logits shape. Expected [batch,3].");
            }

            Dictionary<string, float> scores = new Dictionary<string, float>(aspects.Length);
            for (int row = 0; row < aspects.Length; row++)
            {
                float[] rowLogits = new float[numClasses];
                for (int c = 0; c < numClasses; c++)
                {
                    rowLogits[c] = logitsFlat[row * numClasses + c];
                }

                float[] rowProbs = Softmax(rowLogits);
                float entailmentProbability = rowProbs[ENTAIL_INDEX]; // ENTAIL_INDEX z. B. 2

                scores[aspects[row]] = entailmentProbability;
            }

            return scores.Where(x => x.Value >= _aspectThreshold)
                         .OrderByDescending(static i => i.Value)
                         .Take(_maxReportedAspects)
                         .Select(static x => new ScoreResult { Label = x.Key, Score = x.Value })
                         .ToArray();
        }
        finally
        {
            outputs.Dispose();
        }
    }

    private static float[] Softmax(float[] x)
    {
        float max = x.Max();
        float[] exps = x.Select(v => MathF.Exp(v - max)).ToArray();
        float sum = exps.Sum();

        for (int i = 0; i < exps.Length; i++)
        {
            exps[i] /= sum;
        }

        return exps;
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}