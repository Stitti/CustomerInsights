namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class ApiKeyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LastChars { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Revoked { get; set; }
    }
}
