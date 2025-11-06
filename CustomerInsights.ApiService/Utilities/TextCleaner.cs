using System.Text;
using System.Text.RegularExpressions;

namespace CustomerInsights.ApiService.Utilities;

public static class TextCleaner
{
    // sehr schneller HTML-Stripper (für E-Mail/Docs); für „perfekt“ später AngleSharp/HtmlAgility
    static readonly Regex ReTags = new("<.*?>", RegexOptions.Singleline | RegexOptions.Compiled);
    static readonly Regex ReMultiSpace = new(@"\s{2,}", RegexOptions.Compiled);
    static readonly Regex ReEmailQuote = new(@"(?ms)^\s*On .*? wrote:.*$", RegexOptions.Compiled);
    static readonly Regex ReSignature = new(@"(?ms)(^--\s*$|^Mit freundlichen Grüßen.*$|^Best regards.*$).*", RegexOptions.Compiled);

    public static string Clean(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        var s = input.Replace("\r\n", "\n");
        // HTML raus
        s = ReTags.Replace(s, " ");
        // E-Mail-Ketten abschneiden
        s = ReEmailQuote.Replace(s, "");
        // einfache Signaturen abschneiden
        s = ReSignature.Replace(s, "");
        // Unicode normalisieren
        s = s.Normalize(NormalizationForm.FormKC);
        // Whitespace verdichten
        s = ReMultiSpace.Replace(s, " ").Trim();
        return s;
    }
}
