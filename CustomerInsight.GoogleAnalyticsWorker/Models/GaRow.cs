namespace CustomerInsight.GoogleAnalyticsWorker.Models
{
    public class GaRow
    {
        public string Date { get; set; }
        public string Country { get; set; }
        public long Sessions { get; set; }
        public long TotalUsers { get; set; }
    }
}

