using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CustomerInsights.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;

namespace CustomerInsights.WebhookWorker;

public class WebhookSender
{
    private readonly HttpClient _http;
    private readonly AsyncPolicy<HttpResponseMessage> _policy;
    private readonly AppDbContext _db;
    private readonly string _version;
    private readonly int _timeoutSec;

    public WebhookSender(
        HttpClient http,
        AsyncPolicy<HttpResponseMessage> policy,
        AppDbContext db,
        IOptionsSnapshot<WebhookOptions> opt)
    {
        _http = http;
        _policy = policy;
        _db = db;
        _version = opt.Value.Version ?? "1";
        _timeoutSec = Math.Max(opt.Value.RequestTimeoutSeconds, 5);
    }

    public async Task DispatchAsync(IncomingEvent ev, CancellationToken ct)
    {
        // Finde alle aktiven Endpoints des Tenants, die den Event-Typ abonniert haben
        var endpoints = await _db.WebhookEndpoints
            .Where(e => e.TenantId == ev.TenantId && e.IsActive)
            .ToListAsync(ct);

        foreach (var endpoint in endpoints)
        {
            bool subscribed = string.IsNullOrWhiteSpace(endpoint.EventsCsv)
                ? true
                : endpoint.EventsCsv.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(s => string.Equals(s, ev.Type, StringComparison.OrdinalIgnoreCase));

            if (subscribed == false)
                continue;

            // Idempotenz: Wenn es diese Delivery schon gibt, überspringen
            bool exists = await _db.WebhookDeliveries
                .AnyAsync(d => d.EndpointId == endpoint.Id && d.EventId == ev.EventId, ct);
            if (exists)
                continue;

            WebhookDelivery delivery = new WebhookDelivery
            {
                EndpointId = endpoint.Id,
                EventId = ev.EventId,
                EventType = ev.Type,
                OccurredAt = ev.OccurredAt,
                PayloadJson = JsonSerializer.Serialize(ev.Payload)
            };
            _db.WebhookDeliveries.Add(delivery);
            await _db.SaveChangesAsync(ct);

            _ = SendOneAsync(delivery.Id, ct);
        }
    }

    private async Task SendOneAsync(Guid deliveryId, CancellationToken ct)
    {
        var delivery = await _db.WebhookDeliveries.Include(d => d.Endpoint)
            .FirstAsync(d => d.Id == deliveryId, ct);

        string bodyJson = BuildBody(delivery);
        byte[] bodyBytes = Encoding.UTF8.GetBytes(bodyJson);
        string bodySha = Signatures.Sha256Hex(bodyBytes);

        string id = Guid.NewGuid().ToString();
        string ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        string? signatureHeader = null;
        if (string.IsNullOrEmpty(delivery.Endpoint.HmacSecret) == false)
        {
            string baseString = Signatures.BuildBaseString(ts, id, bodySha);
            string sig = Signatures.HmacSha256Hex(delivery.Endpoint.HmacSecret!, baseString);
            signatureHeader = $"v1={sig}";
        }

        using HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, delivery.Endpoint.Url);
        req.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
        req.Headers.Add("X-Webhook-Id", id);
        req.Headers.Add("X-Webhook-Timestamp", ts);
        req.Headers.Add("X-Webhook-Event", delivery.EventType);
        req.Headers.Add("X-Webhook-Version", _version);
        req.Headers.Add("X-Webhook-Endpoint-Id", delivery.EndpointId.ToString());

        if (string.IsNullOrEmpty(signatureHeader) == false)
            req.Headers.Add("X-Webhook-Signature", signatureHeader);

        if (string.IsNullOrEmpty(delivery.Endpoint.KeyId) == false)
            req.Headers.Add("X-Webhook-Key-Id", delivery.Endpoint.KeyId);

        if (string.IsNullOrEmpty(delivery.Endpoint.BearerToken) == false)
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", delivery.Endpoint.BearerToken);

        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(_timeoutSec));

        try
        {
            HttpResponseMessage resp = await _policy.ExecuteAsync(_ => _http.SendAsync(req, cts.Token), new Context());

            delivery.Attempts += 1;
            delivery.ResponseCode = (int)resp.StatusCode;
            if (resp.IsSuccessStatusCode)
            {
                delivery.Status = "Succeeded";
                delivery.CompletedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                delivery.Status = "Failed";
                delivery.LastError = $"HTTP {(int)resp.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            delivery.Attempts += 1;
            delivery.Status = "Failed";
            delivery.LastError = ex.Message;
        }
        finally
        {
            await _db.SaveChangesAsync(ct);
        }
    }

    private static string BuildBody(WebhookDelivery d)
    {
        var envelope = new
        {
            id = d.EventId.ToString(),
            type = d.EventType,
            occurred_at = d.OccurredAt,
            data = JsonSerializer.Deserialize<object>(d.PayloadJson)
        };
        return JsonSerializer.Serialize(envelope);
    }
}