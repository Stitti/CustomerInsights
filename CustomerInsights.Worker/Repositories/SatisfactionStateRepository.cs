using CustomerInsights.Analytics;
using CustomerInsights.Base.Models;
using CustomerInsights.Database;
using CustomerInsights.Models;
using CustomerInsights.Models.Config;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.InferenceWorker.Repositories;

public sealed class SatisfactionStateRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<SatisfactionStateRepository> _logger;

    public SatisfactionStateRepository(AppDbContext db, ILogger<SatisfactionStateRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Holt (falls vorhanden) den SatisfactionState und baut daraus den Accumulator.
    /// Falls nicht vorhanden, wird ein leerer Accumulator initialisiert.
    /// </summary>
    public async Task<SatisfactionIndexAccumulator> GetAccumulatorAsync(
        Guid tenantId,
        Guid accountId,
        AggregationConfig config,
        Weights weights,
        CancellationToken ct)
    {
        var state = await _db.SatisfactionStates
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.TenantId == tenantId && s.AccountId == accountId, ct);

        if (state is null)
        {
            // Neu initialisieren (entspricht deiner Dapper-Logik)
            return new SatisfactionIndexAccumulator(
                tenantId, accountId, config, weights, DateTime.UtcNow, 0.0d, 0.0d);
        }

        return new SatisfactionIndexAccumulator(
            tenantId, accountId, config, weights,
            state.LastUpdatedUtc, state.DecayedWeightedSum, state.DecayedWeightSum);
    }

    /// <summary>
    /// Upsert: Insert bei Nichtvorhandensein, ansonsten Update (entspricht ON CONFLICT ... DO UPDATE).
    /// </summary>
    public async Task UpsertAccumulatorAsync(Guid tenantId, Guid accountId, SatisfactionIndexAccumulator accumulator, int satisfactionIndex, CancellationToken ct)
    {
        // Mit Composite Key: FindAsync(new object[]{ ... })
        var existing = await _db.SatisfactionStates.FindAsync(new object[] { tenantId, accountId }, ct);

        if (existing is null)
        {
            var entity = new SatisfactionState
            {
                TenantId = tenantId,
                AccountId = accountId,
                LastUpdatedUtc = accumulator.LastUpdatedUtc,
                DecayedWeightedSum = accumulator.DecayedWeightedSum,
                DecayedWeightSum = accumulator.DecayedWeightSum,
                ConfigVersion = accumulator.ConfigVersion,
                SatisfactionIndex = satisfactionIndex
            };

            _db.SatisfactionStates.Add(entity);
            _logger.LogDebug("Inserted satisfaction_state for tenant {TenantId}, account {AccountId}", tenantId, accountId);
        }
        else
        {
            existing.LastUpdatedUtc = accumulator.LastUpdatedUtc;
            existing.DecayedWeightedSum = accumulator.DecayedWeightedSum;
            existing.DecayedWeightSum = accumulator.DecayedWeightSum;
            existing.ConfigVersion = accumulator.ConfigVersion;
            existing.SatisfactionIndex = satisfactionIndex;

            _db.SatisfactionStates.Update(existing);
            _logger.LogDebug("Updated satisfaction_state for tenant {TenantId}, account {AccountId}", tenantId, accountId);
        }

        await _db.SaveChangesAsync(ct);
    }
}