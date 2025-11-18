using CustomerInsights.Base.Enums;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class ChannelCount
    {
        public Channel Channel { get; set; }
        public long InteractionCount { get; set; }
    }
}
