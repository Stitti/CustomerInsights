using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.Base.Models;
using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public AccountListDto? ParentAccount { get; set; }
        public Guid? ParentAccountId { get; set; }
        public string Industry { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public CustomerClassification Classification { get; set; } = CustomerClassification.None;
        public SatisfactionState SatisfactionState { get; set; } = new SatisfactionState();
        public DateTime CreatedAt { get; set; }

        public List<ContactListDto> Contacts { get; set; } = new List<ContactListDto>();
        public List<Interaction> Interactions { get; set; } = new List<Interaction>();

    }
}
