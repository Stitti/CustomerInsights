namespace CustomerInsights.Models
{
    public sealed class AspectRating
    {
        public string AspectName { get; set; } = string.Empty;
        public double Score { get; set; }
        public Guid TextInferenceId { get; set; }
    }
}
