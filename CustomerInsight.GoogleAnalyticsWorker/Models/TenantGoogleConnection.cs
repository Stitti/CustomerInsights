namespace CustomerInsight.GoogleAnalyticsWorker.Models
{
    public sealed class TenantGoogleConnection
    {
        public Guid TenantId { get; init; }
        public string RefreshToken { get; init; } = string.Empty!;
        public string[] Scopes { get; init; } = new[] { "https://www.googleapis.com/auth/analytics.readonly" };
    }
}