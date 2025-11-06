using CustomerInsights.Base.Enums;
using System.Text.Json.Serialization;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class ChannelCountResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Channel Channel { get; set; }
        public string ChannelName { get; set; } = "";
        public long InteractionCount { get; set; }
    }
}
