using CustomerInsights.Base.Enums;

namespace CustomerInsights.Models
{
    public class InteractionEmbedding
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid InteractionId { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? ContactId { get; set; }
        public Channel? Channel { get; set; }
        public string[] Emotions { get; set; } = Array.Empty<string>();
        public string[] Aspects { get; set; } = Array.Empty<string>();
        public string Urgency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string TextFull { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
