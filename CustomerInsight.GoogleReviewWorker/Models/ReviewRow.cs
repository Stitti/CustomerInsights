namespace CustomerInsight.GoogleReview.Models
{
    public sealed class ReviewRow
    {
        public string ReviewId { get; set; } = String.Empty;
        public string ReviewerDisplayName { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset? UpdateTime { get; set; }
    }
}

