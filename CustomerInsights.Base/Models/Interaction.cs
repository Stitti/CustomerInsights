using CustomerInsights.ApiService.Models;
using CustomerInsights.Base.Enums;
using System.Text.Json;

namespace CustomerInsights.Models;

public class Interaction
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Source { get; set; } = "";
    public string ExternalId { get; set; } = "";
    public Channel Channel { get; set; }

    public DateTime OccurredAt { get; set; }
    public DateTime AnalyzedAt { get; set; }

    public Guid? AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }
    public Guid? ThreadId { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string Text { get; set; } = "";

    public JsonDocument? Meta { get; set; }
    public TextInference? TextInference { get; set; }
}

