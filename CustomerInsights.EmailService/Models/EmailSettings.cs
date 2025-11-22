namespace CustomerInsights.EmailService.Models
{
    public sealed class EmailSettings
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
    }
}
