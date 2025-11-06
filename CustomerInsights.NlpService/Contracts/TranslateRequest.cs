namespace CustomerInsights.NlpService.Contracts
{
    public sealed class TranslateRequest
    {
        public string Text { get; set; } = "";
        public string Source { get; set; } = "auto";
        public string Target { get; set; } = "en";
    }
}
