using System.Text;

namespace CustomerInsights.NlpRuntime
{
    public sealed class WordPieceTokenizer
    {
        private readonly Dictionary<string, int> vocabDictionary;
        private readonly bool lowerCase;
        private readonly string unknownToken = "[UNK]";
        private readonly string clsToken = "[CLS]";
        private readonly string sepToken = "[SEP]";

        private static readonly char[] SplitChars = " \t\r\n".ToCharArray();

        public int this[string token]
        {
            get
            {
                int id;
                if (this.vocabDictionary.TryGetValue(token, out id))
                {
                    return id;
                }

                return this.vocabDictionary[this.unknownToken];
            }
        }

        public WordPieceTokenizer(string vocabTxtPath, bool lowerCase = true)
        {
            string[] lines = File.ReadAllLines(vocabTxtPath);

            this.vocabDictionary = lines
                .Select((token, index) => new { Token = token.Trim(), Index = index })
                .ToDictionary(x => x.Token, x => x.Index);

            this.lowerCase = lowerCase;

            if (this.vocabDictionary.ContainsKey(this.unknownToken) == false
                || this.vocabDictionary.ContainsKey(this.clsToken) == false
                || this.vocabDictionary.ContainsKey(this.sepToken) == false)
            {
                throw new InvalidOperationException("vocab.txt must contain [UNK], [CLS], [SEP].");
            }
        }

        public (int[] inputIds, int[] tokenTypeIds, int[] attentionMask) EncodeSingle(
            string text,
            int maxLen = 256)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (this.lowerCase)
            {
                text = text.ToLowerInvariant();
            }

            IEnumerable<string> basicTokens = BasicTokenize(text);
            IEnumerable<string> wordPieces = this.WordPiece(basicTokens);

            List<int> ids = new List<int>(maxLen);
            List<int> tokenTypeIds = new List<int>(maxLen);

            // [CLS]
            ids.Add(this[this.clsToken]);
            tokenTypeIds.Add(0);

            // Tokens
            foreach (string token in wordPieces)
            {
                ids.Add(this[token]);
                tokenTypeIds.Add(0);
            }

            // [SEP]
            ids.Add(this[this.sepToken]);
            tokenTypeIds.Add(0);

            if (ids.Count > maxLen)
            {
                ids = ids.Take(maxLen).ToList();
                tokenTypeIds = tokenTypeIds.Take(maxLen).ToList();
            }

            int[] attentionMask = Enumerable.Repeat(1, ids.Count).ToArray();

            return (ids.ToArray(), tokenTypeIds.ToArray(), attentionMask);
        }
        
        public (int[] inputIds, int[] tokenTypeIds, int[] attentionMask) EncodePair(
            string premise,
            string hypothesis,
            int maxLen = 256)
        {
            if (premise == null)
            {
                throw new ArgumentNullException(nameof(premise));
            }

            if (hypothesis == null)
            {
                throw new ArgumentNullException(nameof(hypothesis));
            }

            if (this.lowerCase)
            {
                premise = premise.ToLowerInvariant();
                hypothesis = hypothesis.ToLowerInvariant();
            }

            IEnumerable<string> premiseTokens = BasicTokenize(premise);
            IEnumerable<string> hypothesisTokens = BasicTokenize(hypothesis);

            IEnumerable<string> wordPiecesPremise = this.WordPiece(premiseTokens);
            IEnumerable<string> wordPiecesHypothesis = this.WordPiece(hypothesisTokens);

            List<int> ids = new List<int>(maxLen);
            List<int> tokenTypeIds = new List<int>(maxLen);

            // [CLS]
            ids.Add(this[this.clsToken]);
            tokenTypeIds.Add(0);

            // Premise
            foreach (string token in wordPiecesPremise)
            {
                ids.Add(this[token]);
                tokenTypeIds.Add(0);
            }

            // [SEP]
            ids.Add(this[this.sepToken]);
            tokenTypeIds.Add(0);

            // Hypothesis
            foreach (string token in wordPiecesHypothesis)
            {
                ids.Add(this[token]);
                tokenTypeIds.Add(1);
            }

            // [SEP]
            ids.Add(this[this.sepToken]);
            tokenTypeIds.Add(1);

            if (ids.Count > maxLen)
            {
                ids = ids.Take(maxLen).ToList();
                tokenTypeIds = tokenTypeIds.Take(maxLen).ToList();
            }

            int[] attentionMask = Enumerable.Repeat(1, ids.Count).ToArray();

            return (ids.ToArray(), tokenTypeIds.ToArray(), attentionMask);
        }

        private static IEnumerable<string> BasicTokenize(string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text.Length);

            foreach (char character in text)
            {
                if (char.IsLetterOrDigit(character))
                {
                    stringBuilder.Append(character);
                }
                else if (char.IsWhiteSpace(character))
                {
                    stringBuilder.Append(' ');
                }
                else
                {
                    stringBuilder.Append(' ');
                    stringBuilder.Append(character);
                    stringBuilder.Append(' ');
                }
            }

            return stringBuilder
                .ToString()
                .Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        private IEnumerable<string> WordPiece(IEnumerable<string> basicTokens)
        {
            foreach (string token in basicTokens)
            {
                if (this.vocabDictionary.ContainsKey(token))
                {
                    yield return token;
                    continue;
                }

                ReadOnlySpan<char> characters = token.AsSpan();
                int start = 0;
                List<string> subTokens = new List<string>();
                bool isBad = false;

                while (start < characters.Length)
                {
                    int end = characters.Length;
                    string? currentSubToken = null;

                    while (start < end)
                    {
                        string piece = new string(characters.Slice(start, end - start));
                        if (start > 0)
                        {
                            piece = "##" + piece;
                        }

                        if (this.vocabDictionary.ContainsKey(piece))
                        {
                            currentSubToken = piece;
                            break;
                        }

                        end--;
                    }

                    if (currentSubToken == null)
                    {
                        isBad = true;
                        break;
                    }

                    subTokens.Add(currentSubToken);
                    start = end;
                }

                if (isBad)
                {
                    yield return this.unknownToken;
                }
                else
                {
                    foreach (string subToken in subTokens)
                    {
                        yield return subToken;
                    }
                }
            }
        }
    }
}