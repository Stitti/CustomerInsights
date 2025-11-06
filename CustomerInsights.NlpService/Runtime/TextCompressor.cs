using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.ML.Tokenizers;

namespace CustomerInsights.NlpService.Runtime;

public static class TextCompressor
{
    private static readonly Regex HtmlTag = new("<.*?>", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex MultiSpace = new(@"\s{2,}", RegexOptions.Compiled);
    private static readonly Regex QuotePrefix = new(@"^\s*>+.*$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex MailHeader = new(@"^(Von|From|Gesendet|Sent|An|To|Betreff|Subject):.*$", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex Signature = new(@"(?ms)(--\s?$|^\s*Mit freundlichen Grüßen.*$|^\s*Best regards.*$).*", RegexOptions.Compiled);
    private static readonly Regex CodeBlock = new(@"```[\s\S]*?```", RegexOptions.Compiled);
    private static readonly Regex InlineCode = new(@"`[^`]+`", RegexOptions.Compiled);
    private static readonly Regex Link = new(@"\[([^\]]+)\]\([^)]+\)", RegexOptions.Compiled);
    private static readonly Regex Image = new(@"!\[([^\]]*)\]\([^)]+\)", RegexOptions.Compiled);
    private static readonly Regex Heading = new(@"^#{1,6}\s*", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex BoldItalic = new(@"(\*\*|__|\*|_)(.*?)\1", RegexOptions.Compiled);
    private static readonly Regex BlockQuote = new(@"^\s*>+\s?", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex TableChars = new(@"(\|)|(:?-+:?)", RegexOptions.Compiled);

    public static string Compress(string text, Tokenizer tokenizer, int maxTokens, string mode = "generic")
    {
        text = HtmlTag.Replace(text, " ");
        text = CodeBlock.Replace(text, " ");
        text = InlineCode.Replace(text, " ");
        text = Link.Replace(text, " ");
        text = Image.Replace(text, " ");
        text = Heading.Replace(text, " ");
        text = BoldItalic.Replace(text, " ");
        text = BlockQuote.Replace(text, " ");
        text = TableChars.Replace(text, " ");
        text = QuotePrefix.Replace(text, " ");
        text = MailHeader.Replace(text, " ");
        text = Signature.Replace(text, " ");
        text = text.Replace("\r", " ").Replace("\n", " ");
        text = MultiSpace.Replace(text, " ").Trim();
        return text;
    }
}