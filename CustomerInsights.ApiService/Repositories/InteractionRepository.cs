using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.Database;               // falls du hier deine DbContext-Options hältst – sonst entfernen
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.ApiService.Database.Repositories;

public sealed class InteractionRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<InteractionRepository> _logger;

    public InteractionRepository(AppDbContext db, ILogger<InteractionRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Interaction?> GetInteractionByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Interactions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.TextInference)
                .ThenInclude(ti => ti.Aspects)
            .Include(i => i.TextInference)
                .ThenInclude(ti => ti.Emotions)
            .SingleOrDefaultAsync(i => i.Id == id, ct);

        return entity;
    }

    public async Task<IEnumerable<Interaction>> GetAllInteractionsAsync(CancellationToken ct = default)
    {
        var list = await _db.Interactions
            .AsNoTracking()
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(ct);

        return list;
    }

    public async Task<Guid> InsertAsync(Interaction interaction, CancellationToken ct = default)
    {
        // IDs/Zeitstempel setzen, falls leer
        if (interaction.Id == Guid.Empty)
            interaction.Id = Guid.NewGuid();

        if (interaction.OccurredAt == default)
            interaction.OccurredAt = DateTime.UtcNow;

        _db.Interactions.Add(interaction);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Inserted interaction {Id}", interaction.Id);
        return interaction.Id;
    }

    /// <summary>
    /// Fügt mehrere Interactions in einem Save ein und gibt deren Ids zurück.
    /// </summary>
    public async Task<Guid[]> InsertBatchAsync(IEnumerable<Interaction> interactions, CancellationToken ct = default)
    {
        // defensive copy + IDs/Zeitstempel setzen
        var list = interactions?.ToList() ?? new List<Interaction>();
        foreach (var i in list)
        {
            if (i.Id == Guid.Empty) i.Id = Guid.NewGuid();
            if (i.OccurredAt == default) i.OccurredAt = DateTime.UtcNow;
        }

        await _db.Interactions.AddRangeAsync(list, ct);
        await _db.SaveChangesAsync(ct);

        var ids = list.Select(i => i.Id).ToArray();
        _logger.LogInformation("Inserted {Count} interactions", ids.Length);
        return ids;
    }

    /// <summary>
    /// Aggregiert Top-Kanäle innerhalb eines Zeitfensters (from/to).
    /// Falls from/to nicht gesetzt sind, werden sie aus 'period' relativ zu 'utcNow' abgeleitet.
    /// </summary>
    public async Task<IReadOnlyList<ChannelCount>> GetTopChannelsAsync(Period period, DateTimeOffset utcNow, DateTimeOffset? fromUtc, DateTimeOffset? toUtc, CancellationToken ct = default)
    {
        var (from, to) = ResolveWindow(period, utcNow, fromUtc, toUtc);

        var query = _db.Interactions
            .AsNoTracking()
            .Where(i => i.OccurredAt >= from.UtcDateTime && i.OccurredAt < to.UtcDateTime);

        var result = await query
            .GroupBy(i => i.Channel)
            .Select(g => new ChannelCount
            {
                Channel = g.Key,
                InteractionCount = g.Count()
            })
            .OrderByDescending(x => x.InteractionCount)
            .Take(5)
            .ToListAsync(ct);

        return result;
    }

    private static (DateTimeOffset From, DateTimeOffset To) ResolveWindow(Period period, DateTimeOffset utcNow, DateTimeOffset? fromUtc, DateTimeOffset? toUtc)
    {
        if (fromUtc.HasValue && toUtc.HasValue)
            return (fromUtc.Value, toUtc.Value);

        DateTimeOffset from;
        DateTimeOffset to;

        switch (period)
        {
            // ---- rollierende Zeiträume ----
            case Period.LastWeek:
                to = utcNow;
                from = utcNow.AddDays(-7);
                break;

            case Period.LastMonth:
                to = utcNow;
                from = utcNow.AddDays(-30);
                break;

            case Period.LastYear:
                to = utcNow;
                from = utcNow.AddDays(-365);
                break;

            // ---- Kalenderbasierte Zeiträume ----
            case Period.CalendarWeek:
                // Montag als Wochenstart
                int diff = ((int)utcNow.DayOfWeek + 6) % 7;
                from = new DateTimeOffset(utcNow.Date.AddDays(-diff), TimeSpan.Zero);
                to = from.AddDays(7);
                break;

            case Period.CalendarMonth:
                from = new DateTimeOffset(utcNow.Year, utcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
                to = from.AddMonths(1);
                break;

            case Period.CalendarYear:
                from = new DateTimeOffset(utcNow.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
                to = from.AddYears(1);
                break;

            default:
                // Fallback: letzte 30 Tage
                to = utcNow;
                from = utcNow.AddDays(-30);
                break;
        }

        return (from, to);
    }
}