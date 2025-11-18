
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.Base.Models;
using CustomerInsights.Models;
using CustomerInsights.SignalWorker.Models;

namespace CustomerInsights.ApiService.Models;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public Account? ParentAccount { get; set; }
    public Guid? ParentAccountId { get; set; }
    public string Industry { get; set; } = string.Empty;
    public string Country {  get; set; } = string.Empty;
    public CustomerClassification Classification { get; set; } = CustomerClassification.None;
    public SatisfactionState SatisfactionState { get; set; } = new SatisfactionState();
    public DateTime CreatedAt { get; set; }

    public List<Contact> Contacts { get; set; } = new List<Contact>();
    public List<Interaction> Interactions { get; set; } = new List<Interaction>();
    public List<Signal> Signals { get; set; } = new List<Signal>();

}