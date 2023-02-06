using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;


namespace KickStat;

public static class SessionExtensions
{
    /// <summary>
    /// Removes items by regex pattern
    /// </summary>
    /// <param name="session"></param>
    /// <param name="pattern">regex pattern</param>
    public static void RemoveByPattern(this ISession session, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentOutOfRangeException(nameof(pattern), "Pattern must be non-empty string");

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var keysToRemove = session.Keys
            .Where(key => regex.IsMatch(key))
            .ToArray();

        foreach (var key in keysToRemove)
            session.Remove(key);
    }

    public static string? GetString(this ISession session, string key, string? defaultValue = null)
    {
        string? result;
        if (session.TryGetValue(key, out byte[]? resultBytes) && resultBytes.IsNotNullOrEmpty())
            result = Encoding.UTF8.GetString(resultBytes);
        else
            result = defaultValue;

        return result;
    }

    public static void SetString(this ISession session, string key, string? value = null)
    {
        if (string.IsNullOrEmpty(value))
            return;

        session.Set(key, Encoding.UTF8.GetBytes(value));
    }
}