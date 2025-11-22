using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CustomerInsights.ServiceDefaults.Services;

public sealed class KeyVaultService
{
    private readonly SecretClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<KeyVaultService> _logger;

    public KeyVaultService(SecretClient client, IMemoryCache cache, ILogger<KeyVaultService> logger)
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> GetSecretAsync(string secretName, string? version = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(secretName))
            throw new ArgumentException("secretName must not be empty.", nameof(secretName));

        string cacheKey = $"{secretName}:{version ?? "latest"}";

        if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null)
            return cached;

        try
        {
            KeyVaultSecret secret = version is null
                ? (await _client.GetSecretAsync(secretName, cancellationToken: ct)).Value
                : (await _client.GetSecretAsync(secretName, version, ct)).Value;

            TimeSpan ttl = TimeSpan.FromMinutes(10);
            DateTimeOffset? expires = secret.Properties.ExpiresOn;
            if (expires is DateTimeOffset exp && exp > DateTimeOffset.UtcNow)
            {
                TimeSpan untilExpiry = exp - DateTimeOffset.UtcNow;
                if (untilExpiry < ttl)
                    ttl = TimeSpan.FromSeconds(Math.Max(5, untilExpiry.TotalSeconds / 2));
            }

            _cache.Set(cacheKey, secret.Value, ttl);
            return secret.Value;
        }
        catch (AuthenticationFailedException ex)
        {
            _logger.LogError(ex, "Authentication to Key Vault failed.");
            throw;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            _logger.LogWarning("Secret '{SecretName}' not found.", secretName);
            return null;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Key Vault request failed with status {Status}.", ex.Status);
            throw;
        }
    }
}