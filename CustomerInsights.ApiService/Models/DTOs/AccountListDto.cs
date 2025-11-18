using CustomerInsights.ApiService.Models.Enums;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class AccountListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public CustomerClassification Classification { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? ParentAccountId { get; set; }
        public string? ParentAccountName { get; set; }
    }
}
