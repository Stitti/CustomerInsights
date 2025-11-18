using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.ApiService.Repositories;

namespace CustomerInsights.ApiService.Services
{
    public sealed class MetricsService
    {
        private readonly MetricsRepository _repository;
        private readonly ILogger<MetricsService> _logger;

        public MetricsService(MetricsRepository repository, ILogger<MetricsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<MetricsDto?> GetAccountMetrics(Guid tenantId, Guid accountId, TimeInterval timeInterval, CancellationToken ct = default)
        {
            return await _repository.GetSingleAccountMetricsAsync(tenantId, accountId, timeInterval, ct);
        }

        public async Task<MetricsDto?> GetTenantMetrics(Guid tenantId, TimeInterval timeInterval, CancellationToken ct = default)
        {
            return await _repository.GetTenantMetrics(tenantId, timeInterval, ct);
        }
    }
}
