using System.Text.Json;
using CustomerInsights.Base.Models.Responses;
using CustomerInsights.NlpRuntime;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace CustomerInsights.NlpService.Runtime;

public sealed class UrgencyOnnx3 : IDisposable
{
    private static readonly string[] Labels = ["LOW","MEDIUM","HIGH"];

    private readonly InferenceSession _session;
    private readonly WordPieceTokenizer _tokenizer;
    private readonly int _maxSequenceLength;
    private readonly string _inputIdsName;
    private readonly string? _attentionMaskName;
    private readonly string? _tokenTypeIdsName;

    public string Version { get; }

    public UrgencyOnnx3()
    {
        JsonElement rootElement = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Config", "models.json"))).RootElement;

        JsonElement urgencyElement = rootElement.GetProperty("Urgency");

        string modelRelativePath = urgencyElement.GetProperty("Path").GetString()!;
        string vocabRelativePath = urgencyElement.GetProperty("Vocab").GetString()!;
        Version = urgencyElement.TryGetProperty("Version", out JsonElement versionElement) ? versionElement.GetString() ?? "unknown" : "unknown";
        _maxSequenceLength = urgencyElement.TryGetProperty("MaxSeqLen", out JsonElement maxSeqElement) ? maxSeqElement.GetInt32() : 256;

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

    public ScoreResult Predict(string text)
    {
        (int[] inputIds, int[]? tokenTypeIds, int[]? attentionMask) = EncodeSingleSequence(text);

        int[] tensorShape = new[] { 1, inputIds.Length };
        var inputIdsTensor = new DenseTensor<long>(tensorShape);
        DenseTensor<long>? attentionMaskTensor = (attentionMask is not null) ? new DenseTensor<long>(tensorShape) : null;
        DenseTensor<long>? tokenTypeIdsTensor  = (tokenTypeIds  is not null) ? new DenseTensor<long>(tensorShape) : null;

        for (int i = 0; i < inputIds.Length; i++)
        {
            inputIdsTensor[0, i] = inputIds[i];
            if (attentionMaskTensor is not null && attentionMask is not null)
                attentionMaskTensor[0, i] = attentionMask[i];

            if (tokenTypeIdsTensor is not null && tokenTypeIds is not null)
                tokenTypeIdsTensor[0, i] = tokenTypeIds[i];
        }

        List<NamedOnnxValue> modelInputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor(_inputIdsName, inputIdsTensor)
        };

        if (_attentionMaskName is not null && attentionMaskTensor is not null)
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_attentionMaskName, attentionMaskTensor));

        if (_tokenTypeIdsName is not null && tokenTypeIdsTensor is not null)
            modelInputs.Add(NamedOnnxValue.CreateFromTensor(_tokenTypeIdsName, tokenTypeIdsTensor));

        using var outputs = _session.Run(modelInputs);

        float[] logits = outputs.First().AsEnumerable<float>().ToArray();
        float[] probabilities = Softmax(logits);

        float maxProbability = probabilities.Max();
        int argmaxIndex = Array.IndexOf(probabilities, maxProbability);
        string predictedLabel = Labels[argmaxIndex];

        return new ScoreResult
        {
            Label = predictedLabel,
            Score = maxProbability
        };
    }

    private (int[] inputIds, int[]? tokenTypeIds, int[]? attentionMask) EncodeSingleSequence(string text)
    {
        (int[] inputIds, int[] tokenTypeIds, int[] attentionMask) = _tokenizer.EncodePair(text, string.Empty, _maxSequenceLength);

        if (!_session.InputMetadata.Keys.Any(static k => k.Contains("token_type_ids", StringComparison.Ordinal)))
            tokenTypeIds = null;

        if (!_session.InputMetadata.Keys.Any(static k => k.Contains("attention_mask", StringComparison.Ordinal)))
            attentionMask = null;

        return (inputIds, tokenTypeIds, attentionMask);
    }

    private static float[] Softmax(float[] values)
    {
        float maxValue = values.Max();
        float[] expValues = values.Select(v => MathF.Exp(v - maxValue)).ToArray();
        float sumExpValues = expValues.Sum();

        for (int i = 0; i < expValues.Length; i++)
        {
            expValues[i] /= sumExpValues;
        }

        return expValues;
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}