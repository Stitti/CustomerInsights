using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.EmbeddingService.Repositories
{
    public class InteractionEmbeddingRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<InteractionEmbeddingRepository> _logger;

        public InteractionEmbeddingRepository(AppDbContext db, ILogger<InteractionEmbeddingRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> CheckForExistingEmbeddingAsync(Guid interactionId, Guid tenantId, CancellationToken cancellationToken)
        {
            if (interactionId == Guid.Empty || tenantId == Guid.Empty)
                return false;

           return await _db.InteractionEmbeddings
                .AsNoTracking()
                .AnyAsync(x => x.TenantId == tenantId && x.InteractionId == interactionId, cancellationToken);
        }

        public async Task<bool> CreateInteractionEmbeddingAsync(Interaction interaction, float[] embeddingVector, CancellationToken cancellationToken)
        {
            if (embeddingVector == null || embeddingVector.Length == 0)
            {
                return false;
            }

            InteractionEmbedding embedding = new InteractionEmbedding();
            embedding.InteractionId = interaction.Id;
            embedding.AccountId = interaction.AccountId;
            embedding.ContactId = interaction.ContactId;
            embedding.Channel = interaction.Channel;
            embedding.Emotions = interaction.TextInference?.Emotions?.Select(x => x.Label)?.ToArray() ?? Array.Empty<string>();
            embedding.CreatedAt = interaction.OccurredAt;
            embedding.TextFull = interaction.Text;
            embedding.Embedding = embeddingVector;
            embedding.Aspects = interaction.TextInference?.Aspects?.Select(x => x.AspectName)?.ToArray() ?? Array.Empty<string>();
            embedding.Urgency = interaction.TextInference?.Urgency ?? string.Empty;

            _db.InteractionEmbeddings.Add(embedding);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Created embedding {EmbeddingId} for {InteractionId} on tenant {TenantId}", embedding.Id, interaction.Id, interaction.TenantId);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "DbUpdateException while saving embedding for interaction {InteractionId} for tenant {TenantId}. Possibly inserted concurrently.", interaction.Id, interaction.TenantId);
                _db.ChangeTracker.Clear();
                return false;
            }
        }
    }
}
