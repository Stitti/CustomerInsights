namespace CustomerInsights.SatisfactionIndexService.Models
{
    public sealed class SatisfactionIndexJobMessage
    {
        public Guid TenantId { get; set; }
        public Guid InteractionId { get; set; }
    }
}
