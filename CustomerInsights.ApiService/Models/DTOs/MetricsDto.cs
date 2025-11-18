namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class MetricsDto
    {
        public int TotalInteractions { get; init; }
        public int TotalInteractionsTrend { get; init; }

        public int TotalHighUrgency { get; init; }
        public int TotalHighUrgencyTrend { get; init; }

        public int ModelConfidence { get; init; }
        public int ModelConfidenceTrend { get; init; }

        public int SatisfactionIndex { get; init; }
        public int SatisfactionIndexTrend { get; init; }
    }
}
