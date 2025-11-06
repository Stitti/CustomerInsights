namespace CustomerInsights.DocumentService.Models
{
    public sealed class EmlMessage
    {
        public string Subject { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string[] To { get; set; } = Array.Empty<string>();
        public string[] Cc { get; set; } = Array.Empty<string>();
        public DateTimeOffset? Date { get; set; }
        public string TextBody { get; set; } = string.Empty;
    }
}