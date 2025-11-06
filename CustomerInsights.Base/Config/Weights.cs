namespace CustomerInsights.Models.Config
{
    public sealed record Weights(double Sentiment = 0.60, double Emotion = 0.25, double Urgency = 0.15);

}
