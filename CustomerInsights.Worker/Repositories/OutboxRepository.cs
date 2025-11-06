using CustomerInsights.Base.Models;
using CustomerInsights.Database;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.InferenceWorker.Repositories
{
    public sealed class OutboxRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<OutboxRepository> _logger;

        public OutboxRepository(AppDbContext db, ILogger<OutboxRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<OutboxMessage?> FetchNextOutboxAsync(string type, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var next = await _db.OutboxMessages
                .FromSqlInterpolated($@"
                    SELECT id, tenant_id AS ""TenantId"", type, target_id AS ""TargetId"",
                           payload_json AS ""PayloadJson"", created_utc AS ""CreatedUtc"",
                           processed_utc AS ""ProcessedUtc"", retry_count AS ""RetryCount"",
                           error_message AS ""ErrorMessage""
                    FROM outbox_messages
                    WHERE processed_utc IS NULL AND type = {type}
                    ORDER BY created_utc
                    LIMIT 1
                    FOR UPDATE SKIP LOCKED")
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            await tx.CommitAsync(ct);
            return next;
        }

        public async Task AckOutboxAsync(Guid id, CancellationToken ct = default)
        {
            var utcNow = DateTime.UtcNow;

            await _db.OutboxMessages
                .Where(m => m.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.ProcessedUtc, utcNow), ct);

            _logger.LogInformation("Outbox {OutboxId} acked at {When}", id, utcNow);
        }

        public async Task NackOutboxAsync(Guid id, string errorMessage, CancellationToken ct = default)
        {
            await _db.OutboxMessages
                .Where(m => m.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.RetryCount, m => m.RetryCount + 1)
                    .SetProperty(m => m.ErrorMessage, errorMessage), ct);
            _logger.LogWarning("Outbox {OutboxId} nacked: {Error}", id, errorMessage);
        }
    }
}