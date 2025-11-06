namespace CustomerInsights.ApiService.Models;

public sealed class ImpactWeight
{
    public Guid TenantId { get; set; }    // PK

    // Gewichte (Defaults: wu=.6, wv=.8, wa=.6)
    public double Wu { get; set; } = 0.6; // Urgency
    public double Wv { get; set; } = 0.8; // Severity
    public double Wa { get; set; } = 0.6; // Aspect-Importance

    // Kappung des Gesamtgewichts (Defaults: 0.5..2.5)
    public double Wmin { get; set; } = 0.5;
    public double Wmax { get; set; } = 2.5;

    // Optional: Versionierung / UpdatedAt
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

