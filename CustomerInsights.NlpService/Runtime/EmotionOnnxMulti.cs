using System.Text.Json;
using CustomerInsights.Base.Models.Responses;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CustomerInsights.NlpService.Runtime;

public sealed class EmotionOnnxMulti : IDisposable
{
    private static readonly string[] Emotions = [
        "frustration", "disappointment", "excitement", "concern",
        "anger", "joy", "confusion", "relief"
    ];

    private static readonly Dictionary<string, string[]> EmotionsMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["frustration"] = new[] { "annoyance", "anger", "disgust", "disappointment" },
        ["disappointment"] = new[] { "disappointment", "sadness", "grief" },
        ["excitement"] = new[] { "excitement", "amusement", "optimism", "desire" },
        ["concern"] = new[] { "fear", "nervousness", "remorse" },
        ["anger"] = new[] { "anger", "disgust", "disapproval" },
        ["joy"] = new[] { "joy", "admiration", "gratitude", "love", "pride" },
        ["confusion"] = new[] { "confusion", "realization", "surprise", "curiosity" },
        ["relief"] = new[] { "relief" }
    };

    private readonly InferenceSession _session;
    private readonly WordPieceTokenizer _tokenizer;
    private readonly int _maxSequenceLength;
    private readonly string _inputIdsName;
    private readonly string? _attentionMaskName;
    private readonly string? _tokenTypeIdsName;
    private readonly float _threshold;
    private readonly int _topK;

    private readonly string[] _sourceLabels;

    public string Version { get; }

    public EmotionOnnxMulti()
    {
        JsonElement root = JsonDocument
            .Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Config", "models.json")))
            .RootElement;

        JsonElement emotion = root.GetProperty("Emotion");

        string modelRelativePath = emotion.GetProperty("Path").GetString()!;
        string vocabRelativePath = emotion.GetProperty("Vocab").GetString()!;
        Version = emotion.TryGetProperty("Version", out var vEl) ? vEl.GetString() ?? "unknown" : "unknown";
        _maxSequenceLength = emotion.TryGetProperty("MaxSeqLen", out var maxEl) ? maxEl.GetInt32() : 256;

        _threshold = emotion.TryGetProperty("Threshold", out var thEl) ? (float)thEl.GetDouble() : 0.5f;
        _topK = emotion.TryGetProperty("TopK", out var tkEl) ? tkEl.GetInt32() : 3;

        // Quell-Labels laden (empfohlen aus Config). Fallback: bekannte GoEmotions-Reihenfolge (28 inkl. "neutral").
        _sourceLabels = TryLoadSourceLabelsFromConfig(emotion);

        string modelPath = Path.Combine(AppContext.BaseDirectory, modelRelativePath);
        string vocabPath = Path.Combine(AppContext.BaseDirectory, vocabRelativePath);

        _tokenizer = new WordPieceTokenizer(vocabPath, lowerCase: true);
        _session = new InferenceSession(modelPath);

        string[] inputNames = _session.InputMetadata.Keys.ToArray();

        _inputIdsName = inputNames.FirstOrDefault(k => k.Contains("input_ids", StringComparison.Ordinal))
                        ?? throw new InvalidOperationException("ONNX-Modell enthält keinen Input namens 'input_ids'.");

        _attentionMaskName = inputNames.FirstOrDefault(k => k.Contains("attention_mask", StringComparison.Ordinal));
        _tokenTypeIdsName = inputNames.FirstOrDefault(k => k.Contains("token_type_ids", StringComparison.Ordinal));
    }

    public ScoreResult[] Score(string text)
    {
        // 1) Encode
        (int[] inputIds, int[]? tokenTypeIds, int[]? attentionMask) = EncodeSingleSequence(text);

        int[] shape = new[] { 1, inputIds.Length };
        var inputIdsTensor = new DenseTensor<long>(shape);
        DenseTensor<long>? attentionMaskTensor = attentionMask is not null ? new(shape) : null;
        DenseTensor<long>? tokenTypeIdsTensor = tokenTypeIds is not null ? new(shape) : null;

        for (int i = 0; i < inputIds.Length; i++)
        {
            inputIdsTensor[0, i] = inputIds[i];
            if (attentionMaskTensor is not null && attentionMask is not null)
                attentionMaskTensor[0, i] = attentionMask[i];
            if (tokenTypeIdsTensor is not null && tokenTypeIds is not null)
                tokenTypeIdsTensor[0, i] = tokenTypeIds[i];
        }

        var modelInputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(_inputIdsName, inputIdsTensor)
        };
        if (_attentionMaskName is not null && attentionMaskTensor is not null)
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_attentionMaskName, attentionMaskTensor));
        if (_tokenTypeIdsName is not null && tokenTypeIdsTensor is not null)
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_tokenTypeIdsName, tokenTypeIdsTensor));

        // 2) Inferenz
        using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputs = _session.Run(modelInputs);
        float[] logits = outputs.First().AsEnumerable<float>().ToArray(); // [num_source_labels]
        float[] probs = Sigmoid(logits);

        // Sanity: Länge muss zu _sourceLabels passen.
        if (probs.Length != _sourceLabels.Length)
        {
            throw new InvalidOperationException(
                $"Output length {probs.Length} != _sourceLabels length {_sourceLabels.Length}. " +
                "Stelle sicher, dass die SourceLabels exakt der Logit-Reihenfolge des Modells entsprechen.");
        }

        // 3) Quell-Scores als Dictionary (LabelName -> Score)
        var srcScores = new Dictionary<string, float>(_sourceLabels.Length, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < _sourceLabels.Length; i++)
            srcScores[_sourceLabels[i]] = probs[i];

        // 4) Auf 8 Ziel-Labels mappen (Aggregation: MAX pro Ziel-Label)
        var tgtScores = new Dictionary<string, float>(Emotions.Length, StringComparer.OrdinalIgnoreCase);
        foreach (string tgt in Emotions)
        {
            if (!EmotionsMap.TryGetValue(tgt, out var srcs) || srcs.Length == 0)
            {
                tgtScores[tgt] = 0f;
                continue;
            }

            float best = 0f;
            foreach (var s in srcs)
            {
                if (srcScores.TryGetValue(s, out var v) && v > best)
                    best = v;
            }
            tgtScores[tgt] = best;
        }

        // 5) TopK + Threshold auswählen
        return tgtScores
            .OrderByDescending(static kv => kv.Value)
            .Take(_topK)
            .Where(kv => kv.Value >= _threshold)
            .Select(x => new ScoreResult { Label = x.Key, Score = x.Value })
            .ToArray();
    }

    private (int[] inputIds, int[]? tokenTypeIds, int[]? attentionMask) EncodeSingleSequence(string text)
    {
        (int[] inputIds, int[] tokenTypeIds, int[] attentionMask) =
            _tokenizer.EncodePair(text, string.Empty, _maxSequenceLength);

        if (!_session.InputMetadata.Keys.Any(k => k.Contains("token_type_ids", StringComparison.Ordinal)))
            tokenTypeIds = null;

        if (!_session.InputMetadata.Keys.Any(k => k.Contains("attention_mask", StringComparison.Ordinal)))
            attentionMask = null;

        return (inputIds, tokenTypeIds, attentionMask);
    }

    private static float[] Sigmoid(float[] x)
    {
        var y = new float[x.Length];
        for (int i = 0; i < x.Length; i++)
            y[i] = 1f / (1f + MathF.Exp(-x[i]));
        return y;
    }

    public void Dispose() => _session.Dispose();

    // -------- Helpers --------

    private static string[]?  TryLoadSourceLabelsFromConfig(JsonElement emotionSection)
    {
        if (emotionSection.TryGetProperty("SourceLabels", out var src))
        {
            var list = src.EnumerateArray().Select(e => e.GetString()!).ToArray();
            if (list.Length > 0) 
                return list;
        }

        if (emotionSection.TryGetProperty("SourceLabelsPath", out var pathEl))
        {
            var pathRel = pathEl.GetString();
            if (!string.IsNullOrWhiteSpace(pathRel))
            {
                var full = Path.Combine(AppContext.BaseDirectory, pathRel!);
                if (File.Exists(full))
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(full));
                    if (doc.RootElement.TryGetProperty("labels", out var arr))
                    {
                        var list = arr.EnumerateArray().Select(e => e.GetString()!).ToArray();
                        if (list.Length > 0) 
                            return list;
                    }
                }
            }
        }

        throw new InvalidOperationException("Found no source labels in config");
    }
}