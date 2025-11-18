using CustomerInsights.EmbeddingService.Contracts;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SessionOptions = Microsoft.ML.OnnxRuntime.SessionOptions;

namespace CustomerInsights.EmbeddingService.Services;

public class OnnxEmbeddingProvider : IDisposable
    {
        private readonly InferenceSession _inferenceSession;
        private readonly WordPieceTokenizerAdapter _textTokenizer;
        private readonly int _maxTokenCount;
        private readonly string _inputIdsName;
        private readonly string _attentionMaskName;
        private readonly string _outputName;

        public OnnxEmbeddingProvider(IConfiguration configuration, WordPieceTokenizerAdapter textTokenizer)
        {
            _textTokenizer = textTokenizer;

            IConfigurationSection embeddingSection = configuration.GetSection("Embedding");

            string? modelPath = embeddingSection.GetValue<string>("OnnxModelPath");
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                throw new InvalidOperationException("Embedding:OnnxModelPath is not configured.");
            }

            _maxTokenCount = embeddingSection.GetValue("MaxTokens", 128);
            _inputIdsName = embeddingSection.GetValue<string>("InputIdsName") ?? "input_ids";
            _attentionMaskName = embeddingSection.GetValue<string>("AttentionMaskName") ?? "attention_mask";
            _outputName = embeddingSection.GetValue<string>("OutputName") ?? "sentence_embedding";

            SessionOptions sessionOptions = new SessionOptions();
            _inferenceSession = new InferenceSession(modelPath, sessionOptions);
        }

        public async Task<EmbedResponse> CreateEmbeddingsAsync(IList<string> texts, string modelName)
        {
            if (texts == null)
            {
                throw new ArgumentNullException(nameof(texts));
            }

            List<EmbedVector> embeddingVectors = new List<EmbedVector>();
            int? embeddingDimension = null;

            for (int textIndex = 0; textIndex < texts.Count; textIndex++)
            {
                string text = texts[textIndex];
                TokenizationResult tokenizationResult = await _textTokenizer.TokenizeAsync(text, _maxTokenCount);
                int[] inputIds = tokenizationResult.InputIds;
                int[] attentionMask = tokenizationResult.AttentionMask;

                if (inputIds.Length != _maxTokenCount || attentionMask.Length != _maxTokenCount)
                {
                    throw new InvalidOperationException("Tokenizer must return arrays with length equal to MaxTokens.");
                }

                DenseTensor<int> inputIdsTensor = new DenseTensor<int>(new[] { 1, _maxTokenCount });
                DenseTensor<int> attentionMaskTensor = new DenseTensor<int>(new[] { 1, _maxTokenCount });

                for (int tokenIndex = 0; tokenIndex < _maxTokenCount; tokenIndex++)
                {
                    inputIdsTensor[0, tokenIndex] = inputIds[tokenIndex];
                    attentionMaskTensor[0, tokenIndex] = attentionMask[tokenIndex];
                }

                List<NamedOnnxValue> inputValues = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor(_inputIdsName, inputIdsTensor),
                    NamedOnnxValue.CreateFromTensor(_attentionMaskName, attentionMaskTensor)
                };

                IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _inferenceSession.Run(inputValues);

                try
                {
                    DisposableNamedOnnxValue? outputValue = null;

                    foreach (DisposableNamedOnnxValue namedValue in results)
                    {
                        if (string.Equals(namedValue.Name, _outputName, StringComparison.Ordinal))
                        {
                            outputValue = namedValue;
                            break;
                        }
                    }

                    if (outputValue == null)
                    {
                        throw new InvalidOperationException(string.Format("ONNX output '{0}' not found in model outputs.", _outputName));
                    }

                    Tensor<float> outputTensor = (Tensor<float>)outputValue.Value;

                    int dimension = outputTensor.Dimensions[1];
                    if (!embeddingDimension.HasValue)
                    {
                        embeddingDimension = dimension;
                    }

                    double[] vectorValues = new double[dimension];
                    for (int dimensionIndex = 0; dimensionIndex < dimension; dimensionIndex++)
                    {
                        float value = outputTensor[0, dimensionIndex];
                        vectorValues[dimensionIndex] = value;
                    }

                    EmbedVector embedVector = new EmbedVector
                    {
                        Index = textIndex,
                        Vector = vectorValues
                    };

                    embeddingVectors.Add(embedVector);
                }
                finally
                {
                    results.Dispose();
                }
            }

            return new EmbedResponse
            {
                Embeddings = embeddingVectors,
                Model = modelName,
                Dimension = embeddingDimension.HasValue ? embeddingDimension.Value : 0
            };

        }

        public void Dispose()
        {
            _inferenceSession.Dispose();
        }
    }