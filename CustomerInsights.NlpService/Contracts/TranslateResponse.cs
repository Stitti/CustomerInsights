namespace CustomerInsights.NlpService.Contracts
{
    public sealed class TranslateResponse
    {
        public string? Text { get; set; }   // übersetzter Text
        public string? Detected { get; set; } // optional: vom MT ermittelte Sprache
    }
}
