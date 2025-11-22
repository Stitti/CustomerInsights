using CustomerInsights.ApiService.Models;

namespace CustomerInsights.Models;

public sealed class Contact
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }
    public List<Interaction> Interactions { get; set; } = new List<Interaction>();
    public DateTime CreatedAt { get; set; }
}