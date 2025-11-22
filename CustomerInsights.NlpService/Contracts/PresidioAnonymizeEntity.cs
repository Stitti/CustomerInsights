using Newtonsoft.Json;

namespace CustomerInsights.NlpService.Contracts
{
    public sealed class PresidioAnonymizeEntity
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("new_value")]
        public string AnonymizedValue { get; set; } = string.Empty;
    }
}
