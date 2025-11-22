using CustomerInsights.Database;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.SignalWorker.Repositories;

public sealed class SatisfactionStateRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<SatisfactionStateRepository> _logger;

    public SatisfactionStateRepository(AppDbContext db, ILogger<SatisfactionStateRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<double> GetAccountSiAsync(Guid tenantId, Guid accountId, CancellationToken ct = default)
    {
        // minimale Projection – lädt NUR die benötigte Spalte
        var si = await _db.SatisfactionStates
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.AccountId == accountId)
            .Select(s => (double?)s.SatisfactionIndex)
            .SingleOrDefaultAsync(ct);

        if (si is null)
        {
            _logger.LogDebug("No satisfaction_state found for tenant {Tenant} / account {Account}. Using default 50.0.", tenantId, accountId);
            return 50.0d;
        }

        return si.Value;
    }
}