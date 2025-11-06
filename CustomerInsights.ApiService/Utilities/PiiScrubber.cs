using System.Text.RegularExpressions;

namespace CustomerInsights.ApiService.Utilities
{
    public record PiiPolicy(bool MaskEmails = true, bool MaskPhones = true, bool MaskIban = true, bool KeepDomain = true);

    public sealed class PiiScrubber
    {
        // einfache, robuste Patterns (keine 100% IBAN-Validierung)
        static readonly Regex ReEmail = new(@"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex RePhone = new(@"\+?[0-9][0-9\-\s()]{6,}[0-9]", RegexOptions.Compiled);
        static readonly Regex ReIban  = new(@"\b[A-Z]{2}[0-9]{2}[A-Z0-9]{10,30}\b", RegexOptions.Compiled);

        private readonly byte[] _hmacKey;
        private readonly PiiPolicy _policy;

        public PiiScrubber(byte[] hmacKey, PiiPolicy? policy = null)
        {
            _hmacKey = hmacKey;
            _policy = policy ?? new();
        }

        public (string CleanText, List<(string Kind, string Raw, string Hash)> Found) Scrub(string text)
        {
            var found = new List<(string,string,string)>();
            var s = text;

            if (_policy.MaskEmails)
            {
                s = ReEmail.Replace(s, m =>
                {
                    var raw = m.Value;
                    var hash = Crypto.HmacSha256(raw.ToLowerInvariant(), _hmacKey);
                    found.Add(("email", raw, hash));
                    if (_policy.KeepDomain && raw.Contains('@'))
                    {
                        var domain = raw.Split('@')[1];
                        return $"***@{domain}";
                    }
                    return "***@***";
                });
            }
            if (_policy.MaskPhones)
            {
                s = RePhone.Replace(s, m =>
                {
                    var raw = m.Value;
                    var hash = Crypto.HmacSha256(raw, _hmacKey);
                    found.Add(("phone", raw, hash));
                    return "****(phone)****";
                });
            }
            if (_policy.MaskIban)
            {
                s = ReIban.Replace(s, m =>
                {
                    var raw = m.Value;
                    var hash = Crypto.HmacSha256(raw, _hmacKey);
                    found.Add(("iban", raw, hash));
                    return "****(iban)****";
                });
            }

            return (s, found);
        }
    }
}

