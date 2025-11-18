using CustomerInsights.GoogleFormsWorker.Services;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;

public sealed class FormsIngestWorker : BackgroundService
{
    private readonly TenantRepository _tenants;
    private readonly GoogleTokenService _tokens;
    private readonly FormClient _forms;
    private readonly ResponseSink _sink;
    private readonly ILogger<FormsIngestWorker> _log;
    private readonly TimeSpan _interval;
    private readonly int _maxParallel;
    private readonly int _pageSize;

    private readonly AsyncRetryPolicy _retry = Policy
        .Handle<Exception>(IsTransient)
        .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2 * i + 1));

    public FormsIngestWorker(TenantRepository tenants, GoogleTokenService tokens, FormClient forms, ResponseSink sink, IConfiguration cfg, ILogger<FormsIngestWorker> log)
    {
        _tenants = tenants;
        _tokens = tokens;
        _forms = forms;
        _sink = sink;
        _log = log;
        _interval = TimeSpan.FromSeconds(int.Parse(cfg["Ingest:IntervalSeconds"] ?? "900"));
        _maxParallel = Math.Max(1, int.Parse(cfg["Ingest:MaxParallelTenants"] ?? "4"));
        _pageSize = Math.Clamp(int.Parse(cfg["Ingest:FormsPageSize"] ?? "500"), 1, 5000);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTimeOffset started = DateTimeOffset.UtcNow;
            try
            {
                await IngestAllTenants(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Forms-Ingest Durchlauf fehlgeschlagen.");
            }

            TimeSpan delay = _interval - (DateTimeOffset.UtcNow - started);
            if (delay < TimeSpan.Zero) 
                delay = TimeSpan.Zero;
            
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task IngestAllTenants(CancellationToken ct)
    {
        var tenants = await _tenants.GetActiveTenantsAsync(ct);
        using SemaphoreSlim sem = new SemaphoreSlim(_maxParallel);

        var tasks = tenants.Select(async tenant =>
        {
            await sem.WaitAsync(ct);
            try
            {
                await _retry.ExecuteAsync(token => IngestTenant(tenant, token), ct);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Ingest für Mandant {Tenant} fehlgeschlagen.", tenant.Value);
            }
            finally { sem.Release(); }
        });

        await Task.WhenAll(tasks);
    }

    private async Task IngestTenant(Guid tenantId, CancellationToken ct)
    {
        var conn = await _tenants.GetGoogleConnectionAsync(tenantId, ct);
        if (conn is null) 
            return;

        var accessToken = await _tokens.GetAccessTokenAsync(conn.RefreshToken, conn.Scopes, ct);

        var forms = await _tenants.GetFormsAsync(tenantId, ct);
        foreach (var (formId, lastSynced) in forms)
        {
            DateTimeOffset sinceUtc = lastSynced ?? DateTimeOffset.UtcNow.AddDays(-7); // initialer Backfill: 7 Tage
            string? pageToken = null;
            DateTimeOffset maxSeenTimestamp = sinceUtc;

            do
            {
                var (responses, next) = await _forms.ListResponsesAsync(
                    accessToken, formId, sinceUtc, _pageSize, pageToken, ct);

                if (responses.Any())
                {
                    await _sink.UpsertResponsesAsync(tenantId, formId, responses, ct);
                    // höchstes „Letzte Aktivität“-Datum tracken
                    foreach (FormResponseRow response in responses)
                    {
                        if (response.LastSubmittedTime is { } t && t > maxSeenTimestamp)
                        {
                            maxSeenTimestamp = t;
                        }
                        else if (response.CreateTime > maxSeenTimestamp)
                        {
                            maxSeenTimestamp = response.CreateTime;
                        }
                    }
                }

                pageToken = next;
            }
            while (string.IsNullOrEmpty(pageToken) == false);

            // Nur wenn wir wirklich was Neues gesehen haben, Synctime vorziehen
            if (maxSeenTimestamp > sinceUtc)
                await _tenants.SetFormLastSyncedAsync(tenantId, formId, maxSeenTimestamp, ct);
        }
    }

    private static bool IsTransient(Exception ex)
        => ex is HttpRequestException
           || ex.Message.Contains("429")
           || ex.Message.Contains("deadline", StringComparison.OrdinalIgnoreCase)
           || ex.Message.Contains("temporarily", StringComparison.OrdinalIgnoreCase);
}