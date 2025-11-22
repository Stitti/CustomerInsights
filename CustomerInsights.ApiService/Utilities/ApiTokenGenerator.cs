using System.Security.Cryptography;
using System.Text;

namespace CustomerInsights.ApiService.Utilities
{
    public static class ApiTokenGenerator
    {
        public static string GenerateToken(int byteLength = 32)
        {
            byte[] randomBytes = new byte[byteLength];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static string HashToken(string token)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(token);
            byte[] hashedBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToHexString(hashedBytes);
        }
    }
}
