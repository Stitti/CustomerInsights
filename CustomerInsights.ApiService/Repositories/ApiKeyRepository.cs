using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Patching;
using CustomerInsights.ApiService.Utilities;
using CustomerInsights.Database;
using CustomerInsights.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerInsights.ApiService.Repositories
{
    public sealed class ApiKeyRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiKeyRepository> _logger;

        public ApiKeyRepository(AppDbContext db, ILogger<ApiKeyRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ApiKeyDto[]> GetAllApiKeysAsync(CancellationToken ct)
        {
            return await _db.ApiKeys.AsNoTracking()
                                    .Select(x => new ApiKeyDto
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Description = x.Description,
                                        LastChars = x.LastChars,
                                        TokenCreated = x.TokenCreated,
                                        Duration = x.Duration,
                                        Revoked = x.Revoked
                                    })
                                    .ToArrayAsync(ct);
        }

        public async Task<ApiKeyDto?> GetApiKeyByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.ApiKeys.AsNoTracking()
                                    .Where(x => x.Id == id)
                                    .Select(x => new ApiKeyDto
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Description = x.Description,
                                        LastChars = x.LastChars,
                                        TokenCreated = x.TokenCreated,
                                        Duration = x.Duration,
                                        Revoked = x.Revoked
                                    })
                                    .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> CreateApiTokenAsync(ApiKey apiKey, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(apiKey.Name))
                throw new ArgumentException("API Key name is required.", nameof(apiKey));

            await _db.ApiKeys.AddAsync(apiKey, ct);

            try
            {
                int affectedRows = await _db.SaveChangesAsync(ct);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create API key {ApiKeyName}", apiKey.Name);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {

            try
            {
                int affectedRows = await _db.ApiKeys.Where(x => x.Id == id)
                                                .ExecuteDeleteAsync(ct);

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete API key {ApiKeyId}", id);
                return false;
            }
        }

        public async Task<bool> RevokeAsync(Guid id, CancellationToken ct)
        {
            try
            {
                int affectedRows = await _db.ApiKeys.Where(x => x.Id == id)
                                                    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Revoked, true), ct);

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke API key {ApiKeyId}", id);
                return false;
            }
        }

        internal async Task<string?> RenewAsync(Guid id, CancellationToken ct)
        {
            string newToken = ApiTokenGenerator.GenerateToken();
            string newHash = ApiTokenGenerator.HashToken(newToken);
            string lastChars = newToken[^4..];

            try
            {
                int affectedRows = await _db.ApiKeys.Where(x => x.Id == id)
                                                    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.TokenCreated, DateTime.UtcNow)
                                                                                          .SetProperty(x => x.TokenHash, newHash)
                                                                                          .SetProperty(x => x.LastChars, lastChars), ct);

                return affectedRows > 0 ? newToken : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to renew API key {ApiKeyId}", id);
                return null;
            }
        }

        internal async Task<bool> PatchAsync(Guid id, UpdateApiKeyRequest request, CancellationToken cancellationToken)
        {
            ApiKey? account = await _db.ApiKeys.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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
                _logger.LogError(ex, "Failed to patch API key {ApiKeyId}", id);
                return false;
            }
        }
    }
}
