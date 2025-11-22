using CustomerInsights.Base.Models;
using CustomerInsights.Database;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.SatisfactionIndexService.Repositories;

public sealed class SatisfactionStateRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SatisfactionStateRepository> _logger;

    public SatisfactionStateRepository(AppDbContext dbContext, ILogger<SatisfactionStateRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SatisfactionState?> GetByAccountIdAsync(Guid tenantId, Guid accountId, CancellationToken cancellationToken)
    {
        return await _dbContext.SatisfactionStates
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.AccountId == accountId, cancellationToken);
    }

    public async Task<List<SatisfactionState>> GetAllByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return await _dbContext.SatisfactionStates
            .Where(s => s.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SatisfactionState state, CancellationToken cancellationToken)
    {
        await _dbContext.SatisfactionStates.AddAsync(state, cancellationToken);
    }

    public Task UpdateAsync(SatisfactionState state, CancellationToken cancellationToken)
    {
        _dbContext.SatisfactionStates.Update(state);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}