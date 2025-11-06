namespace CustomerInsights.ApiService.Models.Contracts;

public sealed class CreateContactRequest
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Guid AccountId { get; set; }
}