namespace CustomerInsights.Models
{
    public sealed class ApiKey
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TokenHash { get; set; } = string.Empty;
        public string LastChars { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Revoked { get; set; }
    }
}
