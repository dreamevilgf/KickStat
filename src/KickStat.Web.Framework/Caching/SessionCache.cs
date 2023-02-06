using System.Text.Json;
using KickStat.Json;
using Microsoft.AspNetCore.Http;

namespace KickStat.Caching;

public class SessionCache : ISessionCache
{
    private readonly ISession _session;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new VersionConverter() }
    };

    public SessionCache(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext?.Session == null)
            throw new ArgumentNullException(nameof(httpContextAccessor), "Вы не можете использовать кэш в сессии вне Web-приложения");

        _session = httpContextAccessor.HttpContext.Session;
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        var sessionItem = _session.GetString(key);
        if (string.IsNullOrEmpty(sessionItem))
            return default;

        var value = JsonSerializer.Deserialize<T>(sessionItem, _serializerOptions);
        return value;
    }

    /// <inheritdoc />
    public void Set(string key, object? data, TimeSpan cacheTime)
    {
        if (data == null)
            return;

        string value = JsonSerializer.Serialize(data, _serializerOptions);
        _session.SetString(key, value);
    }

    /// <inheritdoc />
    public void Set(string key, object? data, int cacheTimeSeconds = 120)
    {
        if (data == null)
            return;

        string value = JsonSerializer.Serialize(data, _serializerOptions);
        _session.SetString(key, value);
    }

    /// <inheritdoc />
    public bool IsSet(string key) => _session.TryGetValue(key, out _);

    /// <inheritdoc />
    public void Remove(string key) => _session.Remove(key);

    /// <inheritdoc />
    public void RemoveByPattern(string pattern)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Clear() => _session.Clear();

    public object? this[string key]
    {
        get => Get<object>(key);
        set => Set(key, value);
    }
}