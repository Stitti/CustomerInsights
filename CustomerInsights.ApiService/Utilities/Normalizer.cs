using System.Text.Json;
using CustomerInsights.ApiService.Services;
using CustomerInsights.Base.Enums;
using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Utilities
{
    public record NormalizeConfig(
        string Source,            // "zendesk", "typeform", ...
        Channel Channel,          // Enum
        string KeyPath,           // "$.id"
        string OccurredAtPath,    // "$.created_at"
        string TextPath,          // "$.body" / "$.text"
        string? ThreadExternalPath = null,
        string? ContactEmailPath  = null
    );

    public sealed class Normalizer
    {
        private readonly PiiScrubber _piiScrubber;
        private readonly IIdentityResolver _identityResolver;

        public Normalizer(PiiScrubber piiScrubber, IIdentityResolver identityResolver)
        {
            _piiScrubber = piiScrubber;
            _identityResolver = identityResolver;
        }

        public Interaction Normalize(Guid tenantId, NormalizeConfig cfg, JsonDocument payload)
        {
            JsonElement root = payload.RootElement;

            string externalId = JsonPick.TryGetString(root, cfg.KeyPath) ?? throw new ArgumentException($"KeyPath {cfg.KeyPath} nicht gefunden");
            DateTime occurred = JsonPick.TryGetDate(root, cfg.OccurredAtPath) ?? throw new ArgumentException($"OccurredAtPath {cfg.OccurredAtPath} nicht gefunden");
            string rawText = JsonPick.TryGetString(root, cfg.TextPath) ?? "";
            string clean = TextCleaner.Clean(rawText);

            // PII Scrub + Hashes erfassen (falls du parallel Contact/Account Links pflegen willst)
            var (scrubbed, hits) = _piiScrubber.Scrub(clean);

            // optional: Kontakt/Account aus E-Mail im Payload
            string? email = cfg.ContactEmailPath is null ? null : JsonPick.TryGetString(root, cfg.ContactEmailPath);
            Guid? contactId = _identityResolver.ResolveContactIdFromEmail(email);
            Guid? accountId = _identityResolver.ResolveAccountIdFromEmail(email);

            // Thread auflösen (z. B. ticket_id, threadId)
            Guid? threadId = null;
            if (string.IsNullOrEmpty(cfg.ThreadExternalPath) == false)
            {
                string? threadExt = JsonPick.TryGetString(root, cfg.ThreadExternalPath);
                if (string.IsNullOrWhiteSpace(threadExt) == false)
                    threadId = _identityResolver.ResolveThreadId(cfg.Source, threadExt!);
            }

            return new Interaction {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Source = cfg.Source,
                Channel = cfg.Channel,
                ExternalId = externalId,
                OccurredAt = occurred,
                Text = scrubbed,
                ThreadId = threadId,
                AccountId = accountId,
                ContactId = contactId,
                Meta = BuildMeta(payload, hits)
            };
        }

        private static JsonDocument BuildMeta(JsonDocument original, List<(string Kind,string Raw,string Hash)> hits)
        {
            var obj = new {
                pii = hits.Select(h => new { kind = h.Kind, hash = h.Hash }).ToArray(),
                // falls du original mitschleppen willst: lieber nicht ganz – nur Hash/Keys
                // original = original.RootElement // <— DSGVO: weglassen oder minimieren
            };
            
            return JsonDocument.Parse(JsonSerializer.Serialize(obj));
        }
    }

}

