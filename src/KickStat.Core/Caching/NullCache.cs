namespace KickStat.Caching;

/// <summary>
/// Represents a NopNullCache
/// </summary>
public class NullCache : ICache
{
    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    public T? Get<T>(string key) => default;

    /// <summary>
    /// Adds the specified key and object to the cache.
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="data">Data</param>
    /// <param name="cacheTime">Cache time</param>
    public void Set(string key, object? data, TimeSpan cacheTime)
    {
    }

    /// <summary>
    /// Adds the specified key and object to the cache.
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="data">Data</param>
    /// <param name="cacheTimeSeconds">Время кэша в секундах</param>
    public void Set(string key, object? data, int cacheTimeSeconds = 120)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the value associated with the specified key is cached
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>Result</returns>
    public bool IsSet(string key)
    {
        return false;
    }

    /// <summary>
    /// Removes the value with the specified key from the cache
    /// </summary>
    /// <param name="key">/key</param>
    public void Remove(string key)
    {
    }

    /// <summary>
    /// Removes items by pattern
    /// </summary>
    /// <param name="pattern">pattern</param>
    public void RemoveByPattern(string pattern)
    {
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    public void Clear()
    {
    }

    public object? this[string key]
    {
        get => Get<object>(key);
        set => Set(key, value);
    }
}