using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Repositories;
using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Services
{
    public sealed class AccountService
    {
        private readonly AccountRepository _repository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(AccountRepository repository, ILogger<AccountService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<AccountListDto>> GetAllAccountsAsync()
        {
            return await _repository.GetAllWithParentAsync();
        }

        public async Task<AccountDto?> GetAccountById(Guid id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<Account> CreateAccountAsync(CreateAccountRequest request)
        {
            Account account = new Account
            {
                Name = request.Name,
                ParentAccountId = request.ParentAccountId,
                Classification = request.Classification,
                Industry = request.Industry,
                Country = request.Country,
                CreatedAt = DateTime.UtcNow,
            };

            return await _repository.CreateAsync(account);
        }

        public async Task<bool> PatchAsync(Guid id, UpdateAccountRequest request)
        {
            return await _repository.Patch(id, request);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.Delete(id);
        }
    }
}
