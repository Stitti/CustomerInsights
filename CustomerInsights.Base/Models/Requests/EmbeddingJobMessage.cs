namespace CustomerInsights.Models.Models.Requests
{
    public class EmbeddingJobMessage
    {
        public Guid InteractionId { get; set; }
        public Guid TenantId { get; set; }
    }
}
