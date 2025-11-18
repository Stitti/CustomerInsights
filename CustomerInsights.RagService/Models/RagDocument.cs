namespace CustomerInsights.RagService.Models
{
    public class RagDocument
    {
        public long Id { get; set; }
        public long InteractionId { get; set; }
        public long? CompanyId { get; set; }
        public long? ContactId { get; set; }
        public string? Channel { get; set; }
        public string? Emotion { get; set; }
        public string[]? Products { get; set; }
        public string[]? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TextFull { get; set; } = string.Empty;
    }
}