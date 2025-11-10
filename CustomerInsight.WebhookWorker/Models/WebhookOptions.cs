public class WebhookOptions
{
    public string? Version { get; set; }
    public int RequestTimeoutSeconds { get; set; } = 25;
}