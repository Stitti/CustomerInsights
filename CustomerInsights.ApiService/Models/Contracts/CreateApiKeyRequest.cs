namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class CreateApiKeyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }
}
