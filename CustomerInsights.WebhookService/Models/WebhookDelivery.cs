namespace CustomerInsights.WebhookService.Models
{
    public sealed class WebhookDelivery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EndpointId { get; set; }
        public WebhookEndpoint Endpoint { get; set; } = default!;
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTimeOffset OccurredAt { get; set; }
        public string PayloadJson { get; set; } = string.Empty!;
        public string Status { get; set; } = "Pending"; // Pending, Succeeded, Failed
        public int Attempts { get; set; }
        public int? ResponseCode { get; set; }
        public string? LastError { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompletedAt { get; set; }
    }
}