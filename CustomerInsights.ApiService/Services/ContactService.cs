using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Services;

public sealed class ContactService
{
    private readonly ContactRepository _repository;
    private readonly ILogger<ContactService> _logger;

    public ContactService(ContactRepository repository, ILogger<ContactService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<ContactListDto>> GetAllContactsAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllWithAccountAsync(cancellationToken);
    }

    public async Task<ContactDto?> GetContactById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdWithAllAsync(id, cancellationToken);
    }

    public async Task<Contact> CreateContactAsync(CreateContactRequest request, CancellationToken cancellationToken = default)
    {
        Contact contact = new Contact
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow,
        };

        return await _repository.CreateAsync(contact, request.AccountId.GetValueOrDefault(), cancellationToken);
    }

    public async Task<bool> PatchAsync(Guid id, UpdateContactRequest request, CancellationToken cancellationToken = default)
    {
        return await _repository.PatchAsync(id, request, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.Delete(id, cancellationToken);
    }
}