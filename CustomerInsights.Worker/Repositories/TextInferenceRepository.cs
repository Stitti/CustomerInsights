using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.InferenceWorker.Repositories
{
    public sealed class TextInferenceRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TextInferenceRepository> _logger;

        public TextInferenceRepository(AppDbContext db, ILogger<TextInferenceRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Legt eine TextInference inkl. Aspects & Emotions neu an.
        /// Erwartet: TextInference.InteractionId ist gesetzt und noch nicht vorhanden.
        /// </summary>
        public async Task<Guid> InsertAsync(TextInference inference, CancellationToken ct = default)
        {
            if (inference is null)
                throw new ArgumentNullException(nameof(inference));

            // defensive defaults
            if (inference.InferredAt == default)
                inference.InferredAt = DateTimeOffset.UtcNow;

            // Prüfen, ob für diese InteractionId bereits eine Inference existiert
            var exists = await _db.TextInferences
                .AsNoTracking()
                .AnyAsync(ti => ti.InteractionId == inference.InteractionId, ct);

            if (exists)
                throw new InvalidOperationException($"TextInference for Interaction {inference.InteractionId} already exists.");

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                _db.TextInferences.Add(inference);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                _logger.LogInformation("Inserted TextInference for Interaction {InteractionId}", inference.InteractionId);
                return inference.InteractionId; // PK == InteractionId
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        /// <summary>
        /// Upsert: legt neu an oder aktualisiert Felder & ersetzt Aspects/Emotions.
        /// </summary>
        public async Task<Guid> UpsertAsync(TextInference inference, CancellationToken ct = default)
        {
            if (inference is null)
                throw new ArgumentNullException(nameof(inference));

            if (inference.InferredAt == default)
                inference.InferredAt = DateTimeOffset.UtcNow;

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var existing = await _db.TextInferences
                    .Include(ti => ti.Aspects)
                    .Include(ti => ti.Emotions)
                    .SingleOrDefaultAsync(ti => ti.InteractionId == inference.InteractionId, ct);

                if (existing is null)
                {
                    _db.TextInferences.Add(inference);
                }
                else
                {
                    // Stammdaten aktualisieren
                    existing.Sentiment = inference.Sentiment;
                    existing.SentimentScore = inference.SentimentScore;
                    existing.Urgency = inference.Urgency;
                    existing.UrgencyScore = inference.UrgencyScore;
                    existing.InferredAt = inference.InferredAt;
                    existing.Extra = inference.Extra;

                    // Owned Collections ersetzen (klar & einfach)
                    existing.Aspects.Clear();
                    foreach (var a in inference.Aspects)
                        existing.Aspects.Add(a);

                    existing.Emotions.Clear();
                    foreach (var e in inference.Emotions)
                        existing.Emotions.Add(e);

                    _db.TextInferences.Update(existing);
                }

                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                _logger.LogInformation("Upserted TextInference for Interaction {InteractionId}", inference.InteractionId);
                return inference.InteractionId;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}