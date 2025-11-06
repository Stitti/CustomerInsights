

using CustomerInsights.Base.Enums;

namespace CustomerInsights.ApiService.Utils
{
    public sealed class TextNormalizer
    {
        public string Normalize(string rawText, Channel channel)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return string.Empty;
            }

            string text = rawText;

            // Remove common email signatures
            if (channel == Channel.Email)
            {
                text = RemoveEmailSignature(text);
            }

            // Strip HTML tags (for emails, tickets)
            if (channel is Channel.Email or Channel.Ticket)
            {
                text = StripHtmlTags(text);
            }

            // Normalize whitespace
            text = NormalizeWhitespace(text);

            return text.Trim();
        }

        private static string RemoveEmailSignature(string text)
        {
            // Simple heuristic: remove everything after common signature markers
            string[] markers = { "\n--\n", "\n-- \n", "Sent from my", "Best regards", "Mit freundlichen Grüßen" };

            foreach (string marker in markers)
            {
                int index = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (index > 0)
                {
                    text = text.Substring(0, index);
                    break;
                }
            }

            return text;
        }

        private static string StripHtmlTags(string text)
        {
            // Very simple HTML stripping (for production use a proper library)
            return System.Text.RegularExpressions.Regex.Replace(
                text,
                @"<[^>]+>",
                string.Empty);
        }

        private static string NormalizeWhitespace(string text)
        {
            // Replace multiple spaces/newlines with single space
            return System.Text.RegularExpressions.Regex.Replace(
                text,
                @"\s+",
                " ");
        }
    }
}
