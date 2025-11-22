namespace CustomerInsights.SatisfactionIndexService.Models
{
    public sealed class Weights
    {
        public double Sentiment { get; set; } = 0.60;
        public double Emotion { get; set; } = 0.25;
        public double Urgency { get; set; } = 0.15;
    }

}
