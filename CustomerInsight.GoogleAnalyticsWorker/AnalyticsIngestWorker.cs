using Polly;
using Polly.Retry;
using CustomerInsight.GoogleAnalyticsWorker.Models;
using CustomerInsight.GoogleAnalyticsWorker.Repositories;
using CustomerInsight.GoogleAnalyticsWorker.Services;
using Microsoft.Extensions.Options;

namespace CustomerInsight.GoogleAnalyticsWorker
{
    public sealed class AnalyticsIngestWorker : BackgroundService
    {
        private readonly TenantRepository _tenantRepository;
        private readonly GoogleTokenService _tokenService;
        private readonly GaDataClient _gaClient;
        private readonly WebsiteReportRepository _websiteReportRepository;
        private readonly ILogger<AnalyticsIngestWorker> _logger;
        private readonly TimeSpan _interval;
        private readonly int _maxParallel;

        private readonly AsyncRetryPolicy _retry = Policy.Handle<Exception>(ex => IsTransient(ex))
                                                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2 * i));

        public AnalyticsIngestWorker(TenantRepository tenantRepository, GoogleTokenService tokenService, GaDataClient gaClient, WebsiteReportRepository websiteReportRepository, IOptions<IngestOptions> options,ILogger<AnalyticsIngestWorker> logger)
        {
            _tenantRepository = tenantRepository;
            _tokenService = tokenService;
            _gaClient = gaClient;
            _websiteReportRepository = websiteReportRepository;
            _logger = logger;
            _interval = TimeSpan.FromSeconds(options.Value.IntervalSeconds);
            _maxParallel = Math.Max(1, options.Value.MaxParallelTenants);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Einfacher Loop; in Produktion evtl. Cron, Quartz o.ä.
            while (stoppingToken.IsCancellationRequested == false)
            {
                DateTimeOffset started = DateTimeOffset.UtcNow;
                try
                {
                    await IngestAllTenants(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ingest-Durchlauf fehlgeschlagen.");
                }

                TimeSpan elapsed = DateTimeOffset.UtcNow - started;
                TimeSpan delay = _interval - elapsed;
                if (delay < TimeSpan.Zero)
                    delay = TimeSpan.Zero;

                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task IngestAllTenants(CancellationToken ct)
        {
            var tenants = await _tenantRepository.GetActiveTenantsAsync(ct);
            _logger.LogInformation("Starte Ingest für {Count} Mandanten …", tenants.Count);

            DateOnly day = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)); // „gestern“ (idempotent)

            // Limitierte Parallelität
            using SemaphoreSlim sem = new SemaphoreSlim(_maxParallel);
            var tasks = tenants.Select(async tenantId =>
            {
                await sem.WaitAsync(ct);
                try
                {
                    await _retry.ExecuteAsync(async token =>
                    {
                        await IngestOneTenant(tenantId, day, token);
                    }, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ingest für Mandant {Tenant} fehlgeschlagen.", tenantId);
                }
                finally
                {
                    sem.Release();
                }
            }).ToArray();

            await Task.WhenAll(tasks);
            _logger.LogInformation("Ingest abgeschlossen.");
        }

        private async Task IngestOneTenant(Guid tenantId, DateOnly day, CancellationToken ct)
        {
            var conn = await _tenantRepository.GetGoogleConnectionAsync(tenantId, ct);
            if (conn is null)
            {
                _logger.LogDebug("Mandant {Tenant} hat keine Google-Verbindung.", tenantId);
                return;
            }

            // Access Token je Mandant ziehen
            var accessToken = await _tokenService.GetAccessTokenAsync(conn.RefreshToken, conn.Scopes, ct);

            var properties = await _tenantRepository.GetGa4PropertiesAsync(tenantId, ct);
            foreach (var prop in properties)
            {
                try
                {
                    var rows = await _gaClient.GetDailyReportAsync(accessToken, prop, day, ct);
                    await _websiteReportRepository.UpsertDailyReportAsync(tenantId, prop, day, rows, ct);
                    _logger.LogInformation("OK: {Tenant} {Prop} {Day}", tenantId, prop.Value, day);
                }
                catch (Exception ex) when (IsPermissionIssue(ex))
                {
                    _logger.LogWarning(ex, "Keine Berechtigung für Property {Prop} bei Mandant {Tenant}.", prop.Value, tenantId);
                    // Optional: Property deaktivieren/flaggen
                }
            }
        }

        private static bool IsTransient(Exception ex)
        {
            return ex is HttpRequestException || ex.Message.Contains("429") || ex.Message.Contains("deadline");
        }
        private static bool IsPermissionIssue(Exception ex)
        {
            return ex.Message.Contains("insufficientPermissions", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("permission", StringComparison.OrdinalIgnoreCase);
        }
    }
}