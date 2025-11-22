namespace CustomerInsights.NlpService.Contracts
{

    public sealed class PresidioAnalyzeAndAnonymizeResult
    {
        public string AnonymizedText { get; set; } = string.Empty;

        public List<PresidioAnalyzerEntity> Entities { get; set; } = new List<PresidioAnalyzerEntity>();
    }
}
