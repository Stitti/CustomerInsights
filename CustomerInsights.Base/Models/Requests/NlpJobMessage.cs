namespace CustomerInsights.NlpService.Contracts
{
    public sealed class NlpJobMessage
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
