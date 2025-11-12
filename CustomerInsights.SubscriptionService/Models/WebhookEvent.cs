namespace CustomerInsights.SubscriptionService.Models
{
    public sealed class WebhookEvent
    {
        public string EventId { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
    }
}

