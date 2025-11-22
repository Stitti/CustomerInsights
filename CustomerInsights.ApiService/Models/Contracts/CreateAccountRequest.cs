using CustomerInsights.ApiService.Models.Enums;

namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class CreateAccountRequest
    {
        public string Name { get; set; }
        public string? ExternalId { get; set; }
        public Guid ParentAccountId { get; set; }
        public string? Industry { get; set; }
        public string? Country { get; set; }
        public CustomerClassification Classification { get; set; } = CustomerClassification.None;
    }
}
