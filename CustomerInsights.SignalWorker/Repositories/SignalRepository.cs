using CustomerInsights.Database;
using CustomerInsights.SignalWorker.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.SignalWorker.Repositories
{
    public sealed class SignalRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SignalRepository> _logger;

        public SignalRepository(AppDbContext db, ILogger<SignalRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task InsertSignalAsync(Signal signal, CancellationToken ct = default)
        {
            if (signal == null)
                throw new ArgumentNullException(nameof(signal));

            if (signal.Id == Guid.Empty)
                signal.Id = Guid.NewGuid();

            if (signal.CreatedUtc == default)
                signal.CreatedUtc = DateTime.UtcNow;

            // Prüfe Dedupe-Key, um ON CONFLICT-Äquivalent zu erreichen
            bool exists = await _db.Signals
                .AsNoTracking()
                .AnyAsync(s => s.DedupeKey == signal.DedupeKey, ct);

            if (exists)
            {
                _logger.LogDebug("Signal with dedupe key {DedupeKey} already exists, skipping insert.", signal.DedupeKey);
                return;
            }

            _db.Signals.Add(signal);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation("Inserted signal {Id} ({Type}, Severity={Severity})", signal.Id, signal.Type, signal.Severity);
        }
    }
}