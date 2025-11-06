using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.Database;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.ApiService.Repositories;

public sealed class AccountRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(AppDbContext db, ILogger<AccountRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Account> CreateAsync(Account account, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(account.Name))
            throw new ArgumentException("Account name is required.", nameof(account));

        if (account.ParentAccount != null)
        {
            bool exists = await _db.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Id == account.ParentAccount.Id, ct);

            if (exists == false)
                throw new InvalidOperationException("Parent account not found.");
        }

        if (account.CreatedAt == default)
            account.CreatedAt = DateTime.UtcNow;

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Created new account {Id} ({Name})", account.Id, account.Name);

        return account;
    }

    public async Task<IEnumerable<AccountListDto>> GetAllWithParentAsync(CancellationToken ct = default)
    {
        List<AccountListDto> accounts = await _db.Accounts
            .AsNoTracking()
            .Include(a => a.ParentAccount)
            .Select(a => new AccountListDto
            {
                Id = a.Id,
                Name = a.Name,
                Industry = a.Industry,
                Classification = a.Classification,
                CreatedAt = a.CreatedAt,
                ParentAccountId = a.ParentAccount != null ? a.ParentAccount.Id : (Guid?)null,
                ParentAccountName = a.ParentAccount != null ? a.ParentAccount.Name : null
            })
            .OrderBy(a => a.Name)
            .ToListAsync(ct);

        _logger.LogDebug("Fetched {Count} accounts", accounts.Count);
        return accounts;
    }

    public async Task<Account?> GetByIdWithDetailsAsync(Guid accountId, int interactionsSkip = 0, int interactionsTake = 200, CancellationToken ct = default)
    {
        // Grundquery
        var query = _db.Accounts
            .AsNoTracking()
            .AsSplitQuery()
            .Where(a => a.Id == accountId)
            .Include(a => a.ParentAccount)
            .Include(a => a.Contacts)
            .Include(a => a.SatisfactionState)
            .Include(a => a.Interactions
                .OrderByDescending(i => i.OccurredAt)
                .Skip(interactionsSkip)
                .Take(interactionsTake))
                .ThenInclude(i => i.TextInference)
                    .ThenInclude(ti => ti.Aspects)
            .Include(a => a.Interactions
                .OrderByDescending(i => i.OccurredAt)
                .Skip(interactionsSkip)
                .Take(interactionsTake))
                .ThenInclude(i => i.TextInference)
                    .ThenInclude(ti => ti.Emotions);

        Account? account = await query.SingleOrDefaultAsync(ct);

        _logger.LogDebug("Loaded account {Id} (contacts: {Contacts}, interactions: {Interactions})", account?.Id, account?.Contacts.Count, account?.Interactions.Count);

        return account;
    }
}
