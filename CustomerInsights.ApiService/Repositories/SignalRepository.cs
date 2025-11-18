using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.Database;
using CustomerInsights.Models;
using CustomerInsights.SignalWorker.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.ApiService.Repositories
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

        public async Task<List<SignalDto>> GetAllAsync(CancellationToken ct = default)
        {
            List<SignalDto> signals = await _db.Signals
                .AsNoTracking()
                .Include(c => c.Account)
                .OrderBy(s => s.CreatedUtc)
                .Select(s => new SignalDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    Severity = s.Severity,
                    AccountSatisfactionIndex = s.AccountSatisfactionIndex,
                    TtlDays = s.TtlDays,
                    Threshold = s.Threshold,
                    CreatedAt = s.CreatedUtc,
                    Account = s.Account != null ? new AccountListDto { Id = s.Account.Id, Name = s.Account.Name } : new AccountListDto(),
                })
                .ToListAsync(ct);

            _logger.LogDebug("Fetched {Count} signals", signals.Count);
            return signals;
        }

        public async Task<SignalDto?> GetById(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
                return null;

            SignalDto? signal = await _db.Signals
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Include(c => c.Account)
                .OrderBy(s => s.CreatedUtc)
                .Select(s => new SignalDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    Severity = s.Severity,
                    AccountSatisfactionIndex = s.AccountSatisfactionIndex,
                    TtlDays = s.TtlDays,
                    Threshold = s.Threshold,
                    CreatedAt = s.CreatedUtc,
                    Account = s.Account != null ? new AccountListDto { Id = s.Account.Id, Name = s.Account.Name } : new AccountListDto(),
                })
                .FirstOrDefaultAsync(ct);

            _logger.LogDebug("Loaded signal {Id} (account: {Account})", signal?.Id, signal?.Account?.Name);

            return signal;
        }
    }
}
