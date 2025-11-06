namespace CustomerInsight.GoogleAnalyticsWorker.Models
{
    public sealed class IngestOptions
    {
        public int IntervalSeconds { get; set; } = 900;
        public int MaxParallelTenants { get; set; } = 4;
    }
}