namespace CustomerInsight.GoogleAnalyticsWorker.Repositories
{
    public sealed class TenantRepository
    {
        public Task<IReadOnlyList<Guid>> GetActiveTenantsAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<TenantGoogleConnection?> GetGoogleConnectionAsync(Guid tenantId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<string>> GetGa4PropertiesAsync(Guid tenantId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}