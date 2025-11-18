namespace CustomerInsights.RagService.Models
{
    public class RagRequest
    {
        public string Question { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public string? Product { get; set; }
        public string? Sentiment { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}