using System.Security.Cryptography;
using System.Text;

namespace CustomerInsights.WebhookService.Utils
{

    public static class Signatures
    {
        public static string Sha256Hex(byte[] data)
        {
            return Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
        }

        public static string HmacSha256Hex(string secret, string data)
        {
            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLowerInvariant();
        }

        public static string BuildBaseString(string timestamp, string id, string bodySha)
        {
            return $"{timestamp}.{id}.{bodySha}";
        }
    }
}