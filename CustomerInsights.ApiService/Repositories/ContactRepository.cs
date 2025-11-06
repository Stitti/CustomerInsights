using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

public sealed class ContactRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<ContactRepository> _logger;

    public ContactRepository(AppDbContext db, ILogger<ContactRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Contact> CreateAsync(Contact contact, Guid accountId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(contact.Firstname) && string.IsNullOrWhiteSpace(contact.Lastname))
            throw new ArgumentException("Contact must have a name (Firstname or Lastname).");

        Account? account = await _db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId, ct);

        if (account == null)
            throw new InvalidOperationException($"Account with ID {accountId} not found.");

        contact.Id = contact.Id == Guid.Empty ? Guid.NewGuid() : contact.Id;
        contact.CreatedAt = contact.CreatedAt == default ? DateTime.UtcNow : contact.CreatedAt;

        contact.Account = _db.Accounts.Local.FirstOrDefault(a => a.Id == accountId) ?? account;

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Created new contact {Id} for account {Account}", contact.Id, account.Name);

        return contact;
    }

    public async Task<IReadOnlyList<ContactListDto>> GetAllWithAccountAsync(string? search = null, int skip = 0, int take = 100, CancellationToken ct = default)
    {
        var q = _db.Contacts
            .AsNoTracking()
            .Include(c => c.Account)
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(search) == false)
        {
            string term = search.Trim().ToLower();
            q = q.Where(c =>
                c.Firstname.ToLower().Contains(term) ||
                c.Lastname.ToLower().Contains(term) ||
                c.Email.ToLower().Contains(term));
        }

        List<ContactListDto> contacts = await q
            .OrderBy(c => c.Lastname)
            .ThenBy(c => c.Firstname)
            .Skip(skip)
            .Take(take)
            .Select(c => new ContactListDto
            {
                Id = c.Id,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Email = c.Email,
                Phone = c.Phone,
                CreatedAt = c.CreatedAt,
                AccountId = c.Account.Id,
                AccountName = c.Account.Name
            })
            .ToListAsync(ct);

        _logger.LogDebug("Fetched {Count} contacts", contacts.Count);
        return contacts;
    }

    public async Task<Contact?> GetByIdWithAllAsync(Guid contactId, bool singleQuery = false, bool includeParentAccount = false, CancellationToken ct = default)
    {
        // Basis
        IQueryable<Contact> query = _db.Contacts
            .AsNoTracking()
            .Where(c => c.Id == contactId);

        query = singleQuery ? query.AsSingleQuery() : query.AsSplitQuery();

        query = includeParentAccount
            ? query
                .Include(c => c.Account)
                    .ThenInclude(a => a.ParentAccount)
            : query
                .Include(c => c.Account);

        query = query
            .Include(c => c.Interactions.OrderByDescending(i => i.OccurredAt))
                .ThenInclude(i => i.TextInference)
                    .ThenInclude(ti => ti.Aspects)
            .Include(c => c.Interactions.OrderByDescending(i => i.OccurredAt))
                .ThenInclude(i => i.TextInference)
                    .ThenInclude(ti => ti.Emotions);

        var contact = await query.SingleOrDefaultAsync(ct);

        _logger.LogDebug("Loaded contact {Id} (account: {Account}, interactions: {Cnt})",
            contact?.Id, contact?.Account?.Name, contact?.Interactions.Count);

        return contact;
    }
}
