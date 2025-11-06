using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Models;

public sealed class Thread
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Source { get; set; } = "";     // z.B. "zendesk", "gmail"
    public string ExternalId { get; set; } = ""; // z.B. Ticket-ID, E-Mail-Thread-ID
    public Guid? AccountId { get; set; }
    public string? Subject { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    //letzte Aktivität zur schnelleren Sortierung in UIs
    public DateTimeOffset? LastActivityAt { get; set; }

    public Interaction[] Interactions { get; set; } = Array.Empty<Interaction>();
}

