namespace CustomerInsights.Base.Models.Responses
{
    public sealed class NlpResponse
    {
        public ScoreResult Sentiment { get; set; } = new ScoreResult();
        public ScoreResult Urgency { get; set; } = new ScoreResult();
        public ScoreResult[] Aspects { get; set; } = Array.Empty<ScoreResult>();
        public ScoreResult[] Emotions { get; set; } = Array.Empty<ScoreResult>();
    }

    public sealed class ScoreResult
    {
        public string Label { get; set; } = string.Empty;
        public float Score { get; set; }
    }
}
