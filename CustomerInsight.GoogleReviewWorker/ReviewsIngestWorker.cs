using CustomerInsight.GoogleAnalyticsWorker.Repositories;
using CustomerInsight.GoogleReview.Models;
using CustomerInsight.GoogleReview.Services;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;

namespace CustomerInsight.GoogleAnalyticsWorker
{
    public sealed class ReviewsIngestWorker : BackgroundService
    {
        private readonly TenantRepository _tenants;
        private readonly GoogleTokenService _tokens;
        private readonly BusinessProfileClient _gbp;
        private readonly ReviewSink _sink;
        private readonly ILogger<ReviewsIngestWorker> _log;
        private readonly TimeSpan _interval;
        private readonly int _maxParallel;

        private readonly AsyncRetryPolicy _retry = Policy
            .Handle<Exception>(IsTransient)
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2 * i));

        public ReviewsIngestWorker(TenantRepository tenants, GoogleTokenService tokens, BusinessProfileClient gbp, ReviewSink sink, IConfiguration cfg, ILogger<ReviewsIngestWorker> log)
        {
            _tenants = tenants;
            _tokens = tokens;
            _gbp = gbp;
            _sink = sink;
            _log = log;
            _interval = TimeSpan.FromSeconds(int.Parse(cfg["Ingest:IntervalSeconds"] ?? "900"));
            _maxParallel = Math.Max(1, int.Parse(cfg["Ingest:MaxParallelTenants"] ?? "4"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var start = DateTimeOffset.UtcNow;
                try { await IngestAll(stoppingToken); }
                catch (Exception ex) { _log.LogError(ex, "Review-Ingest fehlgeschlagen."); }

                var delay = _interval - (DateTimeOffset.UtcNow - start);
                if (delay < TimeSpan.Zero) delay = TimeSpan.Zero;
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task IngestAll(CancellationToken ct)
        {
            var tenants = await _tenants.GetActiveTenantsAsync(ct);
            using var sem = new SemaphoreSlim(_maxParallel);

            var tasks = tenants.Select(async tenant =>
            {
                await sem.WaitAsync(ct);
                try
                {
                    await _retry.ExecuteAsync(token => IngestTenant(tenant, token), ct);
                }
                catch (Exception ex)
                {
                    _log.LogWarning(ex, "Ingest für Mandant {Tenant} fehlgeschlagen.", tenant);
                }
                finally { sem.Release(); }
            });

            await Task.WhenAll(tasks);
        }

        private async Task IngestTenant(Guid tenantId, CancellationToken ct)
        {
            var conn = await _tenants.GetGoogleConnectionAsync(tenantId, ct);
            if (conn is null) return;

            var accessToken = await _tokens.GetAccessTokenAsync(conn.RefreshToken, conn.Scopes, ct);

            // 1) Entweder Locations aus Repo …
            var locations = await _tenants.GetSelectedLocationsAsync(tenantId, ct);

            // …oder dynamisch: Accounts -> Locations (falls du nicht vorwählst)
            if (locations.Count == 0)
            {
                var accounts = await _gbp.ListAccountsAsync(accessToken, ct);
                var all = new List<GbpLocation>();
                foreach (var acc in accounts)
                {
                    all.AddRange(await _gbp.ListLocationsAsync(accessToken, acc.Name, ct));
                }
                
                locations = all;
            }

            foreach (var loc in locations)
            {
                string? pageToken = null;
                do
                {
                    var (reviews, next) = await _gbp.ListReviewsAsync(accessToken, loc.Name, pageSize: 100, pageToken, ct);
                    if (reviews.Any())
                        await _sink.UpsertReviewsAsync(tenantId, loc.Name, reviews, ct);
                    pageToken = next;
                }
                while (!string.IsNullOrEmpty(pageToken));
            }
        }

        private static bool IsTransient(Exception ex)
            => ex is HttpRequestException
               || ex.Message.Contains("429")
               || ex.Message.Contains("deadline", StringComparison.OrdinalIgnoreCase)
               || ex.Message.Contains("temporarily", StringComparison.OrdinalIgnoreCase);
    }
}