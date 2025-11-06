using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.InferenceWorker.Repositories;

public sealed class InteractionRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<InteractionRepository> _logger;

    public InteractionRepository(AppDbContext db, ILogger<InteractionRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Detail: Interaction inkl. TextInference (+ Aspects/Emotions).
    /// </summary>
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

    /// <summary>
    /// Übersicht: alle Interaktionen (ohne Includes).
    /// </summary>
    public async Task<IEnumerable<Interaction>> GetAllInteractionsAsync(CancellationToken ct = default)
    {
        return await _db.Interactions
            .AsNoTracking()
            .OrderByDescending(i => i.OccurredAt)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Insert einer Interaction, Id/OccurredAt defaulten bei Bedarf.
    /// </summary>
    public async Task<Guid> InsertAsync(Interaction interaction, CancellationToken ct = default)
    {
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
    /// Batch-Insert (ein Save), gibt alle IDs zurück.
    /// </summary>
    public async Task<Guid[]> InsertBatchAsync(IEnumerable<Interaction> interactions, CancellationToken ct = default)
    {
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
    /// Markiert als analysiert (setzt nur noch AnalyzedAt).
    /// </summary>
    public async Task MarkAnalyzedAsync(Guid interactionId, CancellationToken ct = default)
    {
        var stub = new Interaction { Id = interactionId };
        _db.Attach(stub);

        stub.AnalyzedAt = DateTime.UtcNow;
        _db.Entry(stub).Property(i => i.AnalyzedAt).IsModified = true;

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Interaction {InteractionId} marked as analyzed at {When}", interactionId, stub.AnalyzedAt);
    }

    /// <summary>
    /// (Optional) Upsert der TextInference (PK = InteractionId).
    /// </summary>
    public async Task UpsertTextInferenceAsync(TextInference inference, CancellationToken ct = default)
    {
        var existing = await _db.TextInferences
            .AsTracking()
            .SingleOrDefaultAsync(ti => ti.InteractionId == inference.InteractionId, ct);

        if (existing is null)
        {
            _db.TextInferences.Add(inference);
        }
        else
        {
            // Kopiere Felder, die sich ändern können (Beispiel)
            existing.Sentiment = inference.Sentiment;
            existing.SentimentScore = inference.SentimentScore;
            existing.Urgency = inference.Urgency;
            existing.UrgencyScore = inference.UrgencyScore;
            existing.InferredAt = inference.InferredAt;
            existing.Extra = inference.Extra;

            // Owned Collections: ersetze sinnvoll (hier simpel: clear & add)
            existing.Aspects.Clear();
            foreach (var a in inference.Aspects) existing.Aspects.Add(a);

            existing.Emotions.Clear();
            foreach (var e in inference.Emotions) existing.Emotions.Add(e);
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Upserted TextInference for Interaction {InteractionId}", inference.InteractionId);
    }
}