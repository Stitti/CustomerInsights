using CustomerInsights.Base.Enums;
using System.Text.Json;

namespace CustomerInsights.Models;

public class Interaction
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    public string Source { get; set; } = "";     // System-Quelle (frei)
    public string ExternalId { get; set; } = ""; // ID in der Quelle (für Idempotenz)
    public Channel Channel { get; set; }         // dein bestehendes Enum

    public DateTime OccurredAt { get; set; }  // Zeitpunkt des Ereignisses
    public DateTime AnalyzedAt { get; set; }

    public Guid? AccountId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? ThreadId { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string Text { get; set; } = "";       // bereinigt & PII-maskiert

    public JsonDocument? Meta { get; set; }
    public TextInference TextInference { get; set; } = new TextInference();
}

