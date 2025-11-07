using CustomerInsights.ApiService.Models.Enums;

namespace CustomerInsights.SignalWorker.Models
{
    public sealed class Signal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TenantId { get; set; }
        public Guid AccountId { get; set; }
        public string Type { get; set; } = "si_below_threshold";
        public SeverityLevel Severity { get; set; } = SeverityLevel.Medium;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public int TtlDays { get; set; } = 7;
        public string DedupeKey { get; set; } = string.Empty;

        // Payload
        public double AccountSatisfactionIndex { get; set; }
        public double Threshold { get; set; }
    }
}
