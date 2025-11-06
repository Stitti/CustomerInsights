namespace CustomerInsights.Base.Models
{
    public sealed class SatisfactionState
    {
        public Guid TenantId { get; set; }
        public Guid AccountId { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public int SatisfactionIndex { get; set; }
        public double DecayedWeightedSum { get; set; }
        public double DecayedWeightSum { get; set; }
        public string ConfigVersion { get; set; }
    }
}
