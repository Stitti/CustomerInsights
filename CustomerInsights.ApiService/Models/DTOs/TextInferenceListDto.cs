using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public class TextInferenceListDto
    {
        public Guid Id { get; set; }
        public string Sentiment { get; set; } = "neu";
        public double SentimentScore { get; set; }
        public string Urgency { get; set; }
        public double UrgencyScore { get; set; }

        public DateTimeOffset InferredAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
