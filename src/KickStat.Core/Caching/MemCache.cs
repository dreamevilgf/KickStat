using Microsoft.Extensions.Caching.Memory;


namespace KickStat.Caching;

/// <summary>
/// Represents a custom application MemoryCache
/// </summary>
public class MemCache : IMemCache
{
    private static MemoryCache? CacheObj;

    private static readonly object _lockObj = new();


    public MemCache()
    {
        if (CacheObj == null)
        {
            lock (_lockObj)
            {
                if (CacheObj == null)
                    CacheObj = new MemoryCache(new MemoryCacheOptions());
            }
        }
    }

    public MemCache(IMemoryCache memoryCache)
    {
        if (CacheObj == null)
        {
            lock (_lockObj)
            {
                if (CacheObj == null)
                    CacheObj = (MemoryCache)memoryCache;
            }
        }
    }

    /// <inheritdoc cref="ICache.Get{T}"/>
    public T Get<T>(string key) => CacheObj.Get<T>(key);

    /// <inheritdoc cref="ICache.Set(string,object,System.TimeSpan)"/>
    public void Set(string key, object? data, TimeSpan cacheTime)
    {
        if (data == null || cacheTime.TotalSeconds < 2)
            return;

        CacheObj.Set(key, data, cacheTime);
    }

    /// <inheritdoc cref="ICache.Set(string,object?,int)"/>
    public void Set(string key, object? data, int cacheTimeSeconds = 120) => Set(key, data, TimeSpan.FromSeconds(cacheTimeSeconds));

    /// <inheritdoc cref="ICache.IsSet"/>
    public bool IsSet(string key) => CacheObj.TryGetValue(key, out _);

    /// <inheritdoc cref="ICache.Remove"/>
    public void Remove(string key) => CacheObj.Remove(key);

    /// <inheritdoc cref="ICache.RemoveByPattern"/>
    public void RemoveByPattern(string pattern) => CacheObj.RemoveByPattern(pattern);


    /// <inheritdoc cref="ICache.Clear"/>
    public void Clear() => CacheObj.Compact(1);

    public object? this[string key]
    {
        get => Get<object>(key);
        set => Set(key, value, 3600);
    }
}