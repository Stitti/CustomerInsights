namespace CustomerInsights.ApiService.Models;

public sealed class AspectWeight
{
    public Guid TenantId { get; set; }        // PK-Teil
    public string AspectKey { get; set; } = ""; // PK-Teil, lowercase (z.B. "lieferzeit")
    public double Importance { get; set; }    // 0..1
}

