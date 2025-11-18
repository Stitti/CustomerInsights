using CustomerInsights.ApiService.Models.Enums;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class SignalDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public AccountListDto Account { get; set; } = new AccountListDto();
        public string Type { get; set; } = "si_below_threshold";
        public SeverityLevel Severity { get; set; } = SeverityLevel.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TtlDays { get; set; } = 7;

        // Payload
        public double AccountSatisfactionIndex { get; set; }
        public double Threshold { get; set; }
    }
}
