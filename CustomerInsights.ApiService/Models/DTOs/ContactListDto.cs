namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class ContactListDto
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Guid? AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
    }
}
