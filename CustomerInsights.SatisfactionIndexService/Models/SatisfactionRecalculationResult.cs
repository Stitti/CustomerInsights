namespace CustomerInsights.SatisfactionIndexService.Models
{
    public sealed class SatisfactionRecalculationResult
    {
        public double AccountSatisfactionIndex { get; init; }
        public double TenantSatisfactionIndex { get; init; }
    }
}
