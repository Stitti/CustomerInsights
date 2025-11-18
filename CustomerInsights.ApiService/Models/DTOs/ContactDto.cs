using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public AccountListDto Account { get; set; } = new AccountListDto();
        public List<Interaction> Interactions { get; set; } = new List<Interaction>();
        public DateTime CreatedAt { get; set; }
    }
}
