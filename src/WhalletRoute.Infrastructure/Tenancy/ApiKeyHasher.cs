using System.Security.Cryptography;
using System.Text;

namespace WhalletRoute.Infrastructure.Tenancy;

public static class ApiKeyHasher
{
    public static string Hash(string rawApiKey)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawApiKey));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}