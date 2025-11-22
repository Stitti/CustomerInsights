using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Repositories;
using CustomerInsights.ApiService.Utilities;
using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Services
{
    public sealed class ApiKeyService
    {
        private readonly ApiKeyRepository _repository;
        private readonly ILogger<ApiKeyService> _logger;

        public ApiKeyService(ApiKeyRepository repository, ILogger<ApiKeyService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiKeyDto[]> GetAllApiKeysAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllApiKeysAsync(cancellationToken);
        }

        public async Task<ApiKeyDto?> GetApiKeyByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetApiKeyByIdAsync(id, cancellationToken);
        }

        public async Task<string?> CreateAsync(CreateApiKeyRequest request, CancellationToken cancellationToken = default)
        {
            string token = ApiTokenGenerator.GenerateToken();

            ApiKey apiKey = new ApiKey
            {
                Name = request.Name,
                Description = request.Description,
                TokenHash = ApiTokenGenerator.HashToken(token),
                LastChars = token[^4..],
                TokenCreated = DateTime.UtcNow,
                Duration = request.Duration,
                Revoked = false
            };

            bool success = await _repository.CreateApiTokenAsync(apiKey, cancellationToken);
            return success ? token : null;
        }

        public async Task<bool> RevokeAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.RevokeAsync(id, cancellationToken);
        }

        public async Task<string?> RenewAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.RenewAsync(id, cancellationToken);
        }

        public async Task<bool> PatchAsync(Guid id, UpdateApiKeyRequest request, CancellationToken cancellationToken = default)
        {

            return await _repository.PatchAsync(id, request, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}
