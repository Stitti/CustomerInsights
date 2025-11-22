namespace CustomerInsights.WebhookService.Models
{
    public sealed class IncomingEvent
    {
        public Guid EventId { get; set; }
        public Guid TenantId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTimeOffset OccurredAt { get; set; }
        public object Payload { get; set; }
    }
}