using System.Text.Json;

namespace CustomerInsights.Models;

public sealed class TextInference
{
    public Guid InteractionId { get; set; }      // PK + FK → Interaction.Id

    public string Sentiment { get; set; } = "neu";   // "neg" | "neu" | "pos"
    public double SentimentScore { get; set; }       // 0..1
    public string Urgency { get; set; }
    public double UrgencyScore { get; set; }


    public List<AspectRating> Aspects { get; set; } = new List<AspectRating>();
    public List<EmotionRating> Emotions { get; set; } = new List<EmotionRating>();

    public DateTimeOffset InferredAt { get; set; } = DateTimeOffset.UtcNow;

    public JsonDocument? Extra { get; set; }         // freies JSON (Scores pro Aspekt etc.)
}