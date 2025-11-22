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

        public async Task<IEnumerable<AccountListDto>> GetAllAccountsAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllWithParentAsync(cancellationToken);
        }

        public async Task<AccountDto?> GetAccountById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdWithDetailsAsync(id, cancellationToken);
        }

        public async Task<Account> CreateAccountAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
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

            return await _repository.CreateAsync(account, cancellationToken);
        }

        public async Task<bool> PatchAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken = default)
        {
            return await _repository.PatchAsync(id, request, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}
