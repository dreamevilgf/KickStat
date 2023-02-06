using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace KickStat.Caching;

public class SessionCacheWithTime : ISessionCache
{
    private readonly ISession _session;

    public SessionCacheWithTime(ISessionFeature sessionFeature)
    {
        if (sessionFeature == null)
            throw new ArgumentNullException(nameof(sessionFeature), "Вы не можете использовать кэш в сессии вне Web-приложения");

        _session = sessionFeature.Session;
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    public T? Get<T>(string key)
    {
        var sessionItem = _session.GetString(key);
        var item = sessionItem != null
            ? JsonSerializer.Deserialize<SessionCacheItem>(sessionItem)
            : null;

        if (item == null || DateTime.UtcNow > item.ExpireAt)
        {
            _session.Remove(key);
            return default;
        }

        return (T)item.Data;
    }

    /// <summary>
    /// Adds the specified key and object to the cache.
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="data">Data</param>
    /// <param name="cacheTime">Cache time</param>
    public void Set(string key, object? data, TimeSpan cacheTime)
    {
        if (data == null)
            return;

        string value = JsonSerializer.Serialize(new SessionCacheItem(data, DateTime.UtcNow + cacheTime));
        _session.SetString(key, value);
    }

    /// <summary>
    /// Adds the specified key and object to the cache.
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="data">Data</param>
    /// <param name="cacheTimeSeconds">Время кэша в минутах</param>
    public void Set(string key, object? data, int cacheTimeSeconds = 120)
    {
        if (data == null)
            return;

        string value = JsonSerializer.Serialize(new SessionCacheItem(data, DateTime.UtcNow.AddSeconds(cacheTimeSeconds)));
        _session.SetString(key, value);
    }

    /// <summary>
    /// Gets a value indicating whether the value associated with the specified key is cached
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>Result</returns>
    public bool IsSet(string key)
    {
        var sessionItem = _session.GetString(key);
        if (string.IsNullOrEmpty(sessionItem))
            return false;

        var sessionCacheItem = JsonSerializer.Deserialize<SessionCacheItem>(sessionItem);
        if (sessionCacheItem == null || DateTime.UtcNow > sessionCacheItem.ExpireAt)
        {
            _session.Remove(key);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Removes the value with the specified key from the cache
    /// </summary>
    /// <param name="key">/key</param>
    public void Remove(string key)
    {
        _session.Remove(key);
    }

    /// <summary>
    /// Removes items by pattern
    /// </summary>
    /// <param name="pattern">pattern</param>
    public void RemoveByPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentOutOfRangeException(nameof(pattern), "Pattern must be non-empty string");

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var keysToRemove = _session.Keys
            .Where(key => regex.IsMatch(key))
            .ToArray();

        foreach (var key in keysToRemove)
            Remove(key);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    public void Clear()
    {
        _session.Clear();
    }

    public object? this[string key]
    {
        get => Get<object>(key);
        set => Set(key, value);
    }
}