public class IncomingEvent
{
    public Guid EventId { get; set; }
    public Guid TenantId { get; set; }
    public string Type { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public object Payload { get; set; }
}