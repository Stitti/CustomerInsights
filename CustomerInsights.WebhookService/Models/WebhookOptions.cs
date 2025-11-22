namespace CustomerInsights.WebhookService.Models
{
    public sealed class WebhookOptions
    {
        public string? Version { get; set; }
        public int RequestTimeoutSeconds { get; set; } = 25;
    }
}