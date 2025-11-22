using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Patching;
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

    public async Task<Contact> CreateAsync(Contact contact, Guid accountId, CancellationToken ct)
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

    public async Task<IReadOnlyList<ContactListDto>> GetAllWithAccountAsync(CancellationToken cancellationToken, string? search = null, int skip = 0, int take = 100)
    {
        IQueryable<Contact> query = _db.Contacts
            .AsNoTracking()
            .Include(c => c.Account)
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(search) == false)
        {
            string term = search.Trim().ToLower();
            query = query.Where(c =>
                c.Firstname.ToLower().Contains(term) ||
                c.Lastname.ToLower().Contains(term) ||
                c.Email.ToLower().Contains(term));
        }

        List<ContactListDto> contacts = await query
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
                AccountId =  c.Account != null ? c.Account.Id : null,
                AccountName = c.Account != null ? c.Account.Name: string.Empty
            })
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Fetched {Count} contacts", contacts.Count);
        return contacts;
    }

    public async Task<ContactDto?> GetByIdWithAllAsync(Guid contactId, CancellationToken cancellationToken, bool singleQuery = false, bool includeParentAccount = false)
    {
        if (contactId == Guid.Empty)
            return null;

        IQueryable<ContactDto> query = _db.Contacts
            .AsNoTracking()
            .Where(c => c.Id == contactId)
            .Include(c => c.Account)
            .Include(c => c.Interactions.OrderByDescending(i => i.OccurredAt))
                .ThenInclude(i => i.TextInference)
                    .ThenInclude(ti => ti.Aspects)
            .Include(c => c.Interactions.OrderByDescending(i => i.OccurredAt))
                .ThenInclude(i => i.TextInference)
                    .ThenInclude(ti => ti.Emotions)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                ExternalId = c.ExternalId,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Email = c.Email,
                Phone = c.Phone,
                CreatedAt = c.CreatedAt,
                Account = c.Account != null ? new AccountListDto { Id = c.Account.Id, Name = c.Account.Name } : null,
                Interactions = c.Interactions
            });

        ContactDto? contact = await query.SingleOrDefaultAsync(cancellationToken);
        _logger.LogDebug("Loaded contact {Id} (account: {Account}, interactions: {Cnt})", contact?.Id, contact?.Account?.Name, contact?.Interactions.Count);
        return contact;
    }

    public async Task<bool> PatchAsync(Guid id, UpdateContactRequest request, CancellationToken cancellationToken)
    {
        Contact? contact = await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (contact == null)
            return false;

        contact.ApplyPatch(request);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        int affectedRows = await _db.Contacts.Where(x => x.Id == id)
                                             .ExecuteDeleteAsync(cancellationToken);

        return affectedRows > 0;
    }
}
