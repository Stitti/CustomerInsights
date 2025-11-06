using System.Security.Cryptography;
using System.Text;

namespace CustomerInsights.ApiService.Utilities
{
    public static class Crypto
    {
        public static string HmacSha256(string input, byte[] key)
        {
            using HMACSHA256 hmac = new HMACSHA256(key);
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(input))).ToLowerInvariant();
        }

        public static bool VerifyHmacSha256(string secret, string body, string providedHeader)
        {
            // Erwartetes Format: "sha256=HEX"
            string[] parts = providedHeader.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2 || !parts[0].Equals("sha256", StringComparison.OrdinalIgnoreCase))
                return false;

            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            string hex = Convert.ToHexString(hash).ToLowerInvariant();
            // Timing-safe compare
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hex), Encoding.UTF8.GetBytes(parts[1].ToLowerInvariant()));
        }
    }
}

