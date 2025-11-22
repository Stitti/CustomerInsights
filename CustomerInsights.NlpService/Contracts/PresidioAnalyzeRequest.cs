using Newtonsoft.Json;

namespace CustomerInsights.NlpService.Contracts
{
    public sealed class PresidioAnalyzeRequest
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;

        [JsonProperty("language")]
        public string Language { get; set; } = string.Empty;

        [JsonProperty("entities")]
        public string[] Entities { get; set; } = Array.Empty<string>();
    }
}
