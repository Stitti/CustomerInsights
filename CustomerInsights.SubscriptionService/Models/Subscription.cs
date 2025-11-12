namespace CustomerInsights.SubscriptionService.Models
{

    public sealed class Subscription
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Provider { get; set; } = "stripe";
        public string ProviderCustomerId { get; set; } = string.Empty;
        public string ProviderSubId { get; set; } = string.Empty;
        public string PlanCode { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CurrentPeriodEndUtcUtc { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}