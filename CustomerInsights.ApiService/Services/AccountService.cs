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

        public async Task<Account?> GetAccountById(Guid id)
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
                CreatedAt = DateTime.UtcNow,
            };

            return await _repository.CreateAsync(account);
        }
    }
}
