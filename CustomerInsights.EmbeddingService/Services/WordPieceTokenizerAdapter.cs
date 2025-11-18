using System;
using System.Threading.Tasks;
using CustomerInsights.EmbeddingService.Contracts;
using CustomerInsights.NlpRuntime;
using Microsoft.Extensions.Options;

namespace CustomerInsights.EmbeddingService.Services
{
    public class WordPieceTokenizerAdapter
    {
        private readonly WordPieceTokenizer _wordPieceTokenizer;
        private readonly int _padTokenId;

        public WordPieceTokenizerAdapter(IOptions<WordPieceTokenizerAdapterOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            WordPieceTokenizerAdapterOptions options = optionsAccessor.Value;

            if (string.IsNullOrWhiteSpace(options.VocabFilePath))
            {
                throw new InvalidOperationException("Tokenizer:VocabFilePath is not configured.");
            }

            _wordPieceTokenizer = new WordPieceTokenizer(options.VocabFilePath, options.LowerCase);
            _padTokenId = _wordPieceTokenizer[options.PadToken];
        }

        public Task<TokenizationResult> TokenizeAsync(string text, int maxTokenCount)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (maxTokenCount <= 2)
            {
                throw new ArgumentException("maxTokenCount must be greater than 2 to allow CLS and SEP tokens.");
            }

            int[] inputIdsRaw;
            int[] tokenTypeIdsRaw;
            int[] attentionMaskRaw;

            (inputIdsRaw, tokenTypeIdsRaw, attentionMaskRaw) = _wordPieceTokenizer.EncodeSingle(text, maxTokenCount);

            int[] inputIds = new int[maxTokenCount];
            int[] attentionMask = new int[maxTokenCount];

            int length = inputIdsRaw.Length;
            if (length > maxTokenCount)
            {
                length = maxTokenCount;
            }

            int index;
            for (index = 0; index < length; index++)
            {
                inputIds[index] = inputIdsRaw[index];
                attentionMask[index] = attentionMaskRaw[index];
            }

            for (index = length; index < maxTokenCount; index++)
            {
                inputIds[index] = _padTokenId;
                attentionMask[index] = 0;
            }

            TokenizationResult tokenizationResult = new TokenizationResult
            {
                InputIds = inputIds,
                AttentionMask = attentionMask
            };

            return Task.FromResult(tokenizationResult);
        }
    }
}
