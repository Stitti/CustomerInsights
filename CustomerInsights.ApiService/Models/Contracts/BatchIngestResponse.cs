namespace CustomerInsights.ApiService.Models.Contracts
{
    public sealed class BatchIngestResponse
    {
        public int TotalSubmitted { get; set; }
        public int SuccessfullyQueued { get; set; }
        public int Failed { get; set; }
        public Guid[] InteractionIds { get; set; } = Array.Empty<Guid>();
        public List<string>? Errors { get; set; }
    }
}
