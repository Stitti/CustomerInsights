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

    public async Task<IEnumerable<ContactListDto>> GetAllContactsAsync()
    {
        return await _repository.GetAllWithAccountAsync();
    }

    public async Task<Contact?> GetContactById(Guid id)
    {
        return await _repository.GetByIdWithAllAsync(id);
    }

    public async Task<Contact> CreateContactAsync(CreateContactRequest request)
    {
        Contact contact = new Contact
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow,
        };

        return await _repository.CreateAsync(contact, request.AccountId);
    }
}