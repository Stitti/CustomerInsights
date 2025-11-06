using System.Text;

namespace CustomerInsights.NlpService.Runtime;

public sealed class WordPieceTokenizer
{
    private readonly Dictionary<string, int> _vocab;
    private readonly bool _lower;
    private readonly string _unk = "[UNK]";
    private readonly string _cls = "[CLS]";
    private readonly string _sep = "[SEP]";

    private static readonly char[] Splitters = " \t\r\n".ToCharArray();

    public int this[string token] => _vocab.TryGetValue(token, out var id) ? id : _vocab[_unk];

    public WordPieceTokenizer(string vocabTxtPath, bool lowerCase = true)
    {
        _vocab = File.ReadAllLines(vocabTxtPath)
                     .Select((t, i) => (t, i))
                     .ToDictionary(x => x.t.Trim(), x => x.i);

        _lower = lowerCase;
        if (_vocab.ContainsKey(_unk) == false || _vocab.ContainsKey(_cls) == false || _vocab.ContainsKey(_sep) == false)
            throw new InvalidOperationException("vocab.txt must contain [UNK], [CLS], [SEP].");
    }

    public (int[] inputIds, int[] tokenTypeIds, int[] attentionMask) EncodePair(string premise, string hypothesis, int maxLen = 256)
    {
        if (_lower) 
        { 
            premise = premise.ToLowerInvariant(); 
            hypothesis = hypothesis.ToLowerInvariant(); 
        }

        IEnumerable<string> premTokens = BasicTokenize(premise);
        IEnumerable<string> hypoTokens = BasicTokenize(hypothesis);

        IEnumerable<string> wpPrem = WordPiece(premTokens);
        IEnumerable<string> wpHypo = WordPiece(hypoTokens);

        List<int> ids = new List<int>(maxLen);
        List<int> tt = new List<int>(maxLen);
        ids.Add(this[_cls]); 
        tt.Add(0);

        foreach (string t in wpPrem) 
        { 
            ids.Add(this[t]);
            tt.Add(0); 
        }

        ids.Add(this[_sep]); 
        tt.Add(0);

        foreach (string t in wpHypo) 
        { 
            ids.Add(this[t]); tt.Add(1); 
        }

        ids.Add(this[_sep]);
        tt.Add(1);

        if (ids.Count > maxLen)
        {
            ids = ids.Take(maxLen).ToList();
            tt = tt.Take(maxLen).ToList();
        }

        int[] mask = Enumerable.Repeat(1, ids.Count).ToArray();
        return (ids.ToArray(), tt.ToArray(), mask);
    }

    private static IEnumerable<string> BasicTokenize(string text)
    {
        // sehr einfache Vor-Tokenisierung: whitespace + rudimentäre Zeichentrennung
        StringBuilder stringBuilder = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (char.IsLetterOrDigit(c))
            {
                stringBuilder.Append(c);
            }
            else if (char.IsWhiteSpace(c)) 
            { 
                stringBuilder.Append(' '); 
            }
            else 
            {
                stringBuilder.Append(' '); stringBuilder.Append(c); stringBuilder.Append(' '); 
            }
        }
        return stringBuilder.ToString().Split(Splitters, StringSplitOptions.RemoveEmptyEntries);
    }

    private IEnumerable<string> WordPiece(IEnumerable<string> basicTokens)
    {
        foreach (string token in basicTokens)
        {
            if (_vocab.ContainsKey(token)) { yield return token; continue; }

            ReadOnlySpan<char> chars = token.AsSpan();
            int start = 0;
            List<string> subTokens = new List<string>();
            var isBad = false;

            while (start < chars.Length)
            {
                var end = chars.Length;
                string? cur = null;
                while (start < end)
                {
                    string piece = new string(chars.Slice(start, end - start));
                    if (start > 0) 
                        piece = "##" + piece;

                    if (_vocab.ContainsKey(piece)) 
                    { 
                        cur = piece;
                        break; 
                    }

                    end--;
                }

                if (cur == null) 
                { 
                    isBad = true; 
                    break; 
                }

                subTokens.Add(cur);
                start = end;
            }

            if (isBad)
            {
                yield return _unk;
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
