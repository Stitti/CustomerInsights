using Azure.Core;
using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Patching;
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

    public async Task<Account> CreateAsync(Account account, CancellationToken ct)
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

    public async Task<IEnumerable<AccountListDto>> GetAllWithParentAsync(CancellationToken ct)
    {
        List<AccountListDto> accounts = await _db.Accounts
            .AsNoTracking()
            .Include(a => a.ParentAccount)
            .Select(a => new AccountListDto
            {
                Id = a.Id,
                Name = a.Name,
                Industry = a.Industry,
                Country = a.Country,
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

    public async Task<AccountDto?> GetByIdWithDetailsAsync(Guid accountId, CancellationToken ct, int interactionsSkip = 0, int interactionsTake = 200)
    {
        if (accountId == Guid.Empty)
            return null;

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
                    .ThenInclude(ti => ti.Emotions)
            .Select(a => new AccountDto
            {
                Id = a.Id,
                ExternalId = a.ExternalId,
                Name = a.Name,
                Classification = a.Classification,
                Country = a.Country,
                Industry = a.Industry,
                ParentAccount = a.ParentAccount != null ? new AccountListDto { Id = a.ParentAccount.Id, Name = a.ParentAccount.Name } : null,
                CreatedAt = a.CreatedAt,
                SatisfactionState = a.SatisfactionState,
                Contacts = a.Contacts != null ? a.Contacts.Select(c => new ContactListDto
                {
                    Id = c.Id,
                    Firstname = c.Firstname,
                    Lastname = c.Lastname,
                    Email = c.Email,
                    Phone = c.Phone,
                    CreatedAt = c.CreatedAt
                }).ToList() : new List<ContactListDto>()

            });

        AccountDto? account = await query.SingleOrDefaultAsync(ct);

        _logger.LogDebug("Loaded account {Id} (contacts: {Contacts}, interactions: {Interactions})", account?.Id, account?.Contacts.Count, account?.Interactions.Count);

        return account;
    }

    public async Task<bool> PatchAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        Account? account = await _db.Accounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (account == null)
            return false;

        account.ApplyPatch(request);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to patch account {AccountId}", id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            int affectedRows = await _db.Accounts.Where(x => x.Id == id)
                                                 .ExecuteDeleteAsync(cancellationToken);

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete account {AccountId}", id);
            return false;
        }
    }
}
