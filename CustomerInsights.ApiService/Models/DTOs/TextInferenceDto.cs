using CustomerInsights.Models;
using System.Text.Json;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class TextInferenceDto
    {
        public Guid Id { get; set; }
        public string Sentiment { get; set; } = "neu";
        public double SentimentScore { get; set; }
        public string Urgency { get; set; }
        public double UrgencyScore { get; set; }


        public List<AspectRating> Aspects { get; set; } = new List<AspectRating>();
        public List<EmotionRating> Emotions { get; set; } = new List<EmotionRating>();

        public DateTimeOffset InferredAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
