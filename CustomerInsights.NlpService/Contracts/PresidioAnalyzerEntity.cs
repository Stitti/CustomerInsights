namespace CustomerInsights.NlpService.Contracts
{
    public sealed class PresidioAnalyzerEntity
    {
        public int Start { get; set; }

        public int End { get; set; }

        public string EntityType { get; set; } = string.Empty;

        public double Score { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
