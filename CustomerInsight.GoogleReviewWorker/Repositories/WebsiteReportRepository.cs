using CustomerInsight.GoogleAnalyticsWorker.Models;

namespace CustomerInsight.GoogleAnalyticsWorker.Repositories
{
    public sealed class WebsiteReportRepository
    {
        public Task UpsertDailyReportAsync(Guid tenantId, string propertyId, DateOnly day, IEnumerable<GaRow> rows, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}