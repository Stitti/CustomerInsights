using System.ComponentModel.DataAnnotations;

public class WebhookEndpoint
{
    public Guid TenantId { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = default!;
    public string? Name { get; set; }
    public string? KeyId { get; set; }
    public bool IsActive { get; set; } = true;
    public string Events { get; set; } = string.Empty;
    public int? RateLimitPerMinute { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}