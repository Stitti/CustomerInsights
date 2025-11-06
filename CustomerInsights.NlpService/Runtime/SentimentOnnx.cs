using System.Text.Json;
using CustomerInsights.Base.Models.Responses;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CustomerInsights.NlpService.Runtime;

public sealed class SentimentOnnx3 : IDisposable
{
    private static readonly string[] Labels = [ "NEUTRAL", "POSITIVE", "NEGATIVE" ];

    private readonly InferenceSession _session;
    private readonly WordPieceTokenizer _tokenizer;
    private readonly int _maxSequenceLength;
    private readonly string _inputIdsName;
    private readonly string? _attentionMaskName;
    private readonly string? _tokenTypeIdsName;

    public string Version { get; }

    public SentimentOnnx3()
    {
        JsonElement rootElement = JsonDocument
            .Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Config", "models.json")))
            .RootElement;

        JsonElement sentimentElement = rootElement.GetProperty("Sentiment");

        string modelRelativePath = sentimentElement.GetProperty("Path").GetString()!;
        string vocabRelativePath = sentimentElement.GetProperty("Vocab").GetString()!;
        Version = sentimentElement.TryGetProperty("Version", out JsonElement versionElement) ? versionElement.GetString() ?? "unknown" : "unknown";
        _maxSequenceLength = sentimentElement.TryGetProperty("MaxSeqLen", out JsonElement maxSeqElement) ? maxSeqElement.GetInt32() : 256;

        string modelPath = Path.Combine(AppContext.BaseDirectory, modelRelativePath);
        string vocabPath = Path.Combine(AppContext.BaseDirectory, vocabRelativePath);

        _tokenizer = new WordPieceTokenizer(vocabPath, lowerCase: true);
        _session = new InferenceSession(modelPath);

        string[] inputNames = _session.InputMetadata.Keys.ToArray();

        string? inputIdsName = inputNames.FirstOrDefault(static k => k.Contains("input_ids", StringComparison.Ordinal));
        if (inputIdsName is null)
        {
            throw new InvalidOperationException("ONNX-Modell enthält keinen Input namens 'input_ids'.");
        }

        _inputIdsName = inputIdsName;
        _attentionMaskName = inputNames.FirstOrDefault(static k => k.Contains("attention_mask", StringComparison.Ordinal));
        _tokenTypeIdsName = inputNames.FirstOrDefault(static k => k.Contains("token_type_ids", StringComparison.Ordinal));
    }

    /// <summary>
    /// Führt Sentimentanalyse (3 Klassen) für einen Text aus und gibt Label zurück.
    /// </summary>
    public ScoreResult Predict(string text)
    {
        (int[] inputIds, int[]? tokenTypeIds, int[]? attentionMask) = EncodeSingleSequence(text);

        int[] tensorShape = new[] { 1, inputIds.Length };
        DenseTensor<long> inputIdsTensor = new DenseTensor<long>(tensorShape);
        DenseTensor<long>? attentionMaskTensor = attentionMask is not null ? new DenseTensor<long>(tensorShape) : null;
        DenseTensor<long>? tokenTypeIdsTensor = tokenTypeIds is not null ? new DenseTensor<long>(tensorShape) : null;

        for (int index = 0; index < inputIds.Length; index++)
        {
            inputIdsTensor[0, index] = inputIds[index];
            if (attentionMaskTensor is not null && attentionMask is not null)
            {
                attentionMaskTensor[0, index] = attentionMask[index];
            }

            if (tokenTypeIdsTensor is not null && tokenTypeIds is not null)
            {
                tokenTypeIdsTensor[0, index] = tokenTypeIds[index];
            }
        }

        List<NamedOnnxValue> modelInputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(_inputIdsName, inputIdsTensor)
        };

        if (_attentionMaskName is not null && attentionMaskTensor is not null)
        {
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_attentionMaskName, attentionMaskTensor));
        }

        if (_tokenTypeIdsName is not null && tokenTypeIdsTensor is not null)
        {
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_tokenTypeIdsName, tokenTypeIdsTensor));
        }

        IDisposableReadOnlyCollection<DisposableNamedOnnxValue> inferenceOutputs = _session.Run(modelInputs);
        float[] logits = inferenceOutputs.First().AsEnumerable<float>().ToArray(); // erwartete Form: [1,3]
        float[] probabilities = Softmax(logits);

        float maxProbability = probabilities.Max();
        int argmaxIndex = Array.IndexOf(probabilities, maxProbability);
        string predictedLabel = Labels[argmaxIndex];

        inferenceOutputs.Dispose();

        return new ScoreResult
        {
            Label = predictedLabel,
            Score = maxProbability
        };
    }

    /// <summary>
    /// Kodiert den Text als einzelne Sequenz im BERT-Format: [CLS] text [SEP].
    /// </summary>
    private (int[] inputIds, int[]? tokenTypeIds, int[]? attentionMask) EncodeSingleSequence(string text)
    {
        // Der vorhandene Tokenizer erzeugt per EncodePair(text, "") Sequenz inkl. [CLS], [SEP] und optionalem zweiten [SEP].
        (int[] inputIds, int[] tokenTypeIds, int[] attentionMask) = _tokenizer.EncodePair(text, string.Empty, _maxSequenceLength);

        // Wenn das Modell bestimmte Inputs nicht erwartet, nicht mitsenden.
        if (!_session.InputMetadata.Keys.Any(static k => k.Contains("token_type_ids", StringComparison.Ordinal)))
        {
            tokenTypeIds = null;
        }

        if (!_session.InputMetadata.Keys.Any(static k => k.Contains("attention_mask", StringComparison.Ordinal)))
        {
            attentionMask = null;
        }

        return (inputIds, tokenTypeIds, attentionMask);
    }

    private static float[] Softmax(float[] values)
    {
        float maxValue = values.Max();
        float[] expValues = values.Select(v => MathF.Exp(v - maxValue)).ToArray();
        float sumExpValues = expValues.Sum();

        for (int index = 0; index < expValues.Length; index++)
        {
            expValues[index] /= sumExpValues;
        }

        return expValues;
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}