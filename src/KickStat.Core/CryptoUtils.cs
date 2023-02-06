using System.Security.Cryptography;
using System.Text;


namespace KickStat;

public static class CryptoUtils
{
    public static string GetMD5Hash(string input) => GetHash(input, MD5.Create());

    public static string GetSHA1Hash(string input) => GetHash(input, SHA1.Create());

    public static string GetHMAC256Hash(string input, string signature)
    {
        byte[] key = Encoding.UTF8.GetBytes(signature);
        var hmacsha1 = new HMACSHA1(key);

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = hmacsha1.ComputeHash(inputBytes);
        return Convert.ToBase64String(hash);
    }

    public static string GetHash(string input, HashAlgorithm hashAlgorithm)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        byte[] hashBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        var sBuilder = new StringBuilder(hashBytes.Length * 2);
        foreach (byte b in hashBytes)
            sBuilder.Append(b.ToString("x2"));

        return sBuilder.ToString().ToLowerInvariant();
    }

    public static bool IsValidServiceKey(string key) => GetSHA1Hash(key) == "fccb11aa1a375ec72e433e54e009c05cb1346a57";

    public static bool IsValidServiceKeyHashed(string key) => key == "fccb11aa1a375ec72e433e54e009c05cb1346a57";
}