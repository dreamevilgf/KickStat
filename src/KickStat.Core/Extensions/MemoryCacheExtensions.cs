using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Caching.Memory;


namespace KickStat;

public static partial class MemoryCacheExtensions
{
    private static readonly Type _memoryCacheType = typeof(MemoryCache);

    /// <summary>
    /// Adds the specified key and object to the cache.
    /// </summary>
    /// <param name="memoryCache">IMemoryCache object</param>
    /// <param name="key">Cache key</param>
    /// <param name="data">Data to cache</param>
    /// <param name="cacheTimeSeconds">Cache time in seconds</param>
    public static void Set(this IMemoryCache memoryCache, object key, object data, int cacheTimeSeconds = 120) =>
        memoryCache.Set(key, data, TimeSpan.FromSeconds(cacheTimeSeconds));

    /// <summary>
    /// Gets a value indicating whether the value associated with the specified key is cached
    /// </summary>
    /// <param name="memoryCache">IMemoryCache object</param>
    /// <param name="key">Cache key</param>
    public static bool IsSet(this IMemoryCache memoryCache, string key) =>
        memoryCache.TryGetValue(key, out _);


    /// <summary>
    /// Получить все ключи в кэше.
    /// ВНИМАНИЕ!!! Используется рефлексия!
    /// </summary>
    /// <param name="memoryCache"></param>
    public static object[]? GetAllKeys(this IMemoryCache memoryCache)
    {
        var memCache = memoryCache as MemoryCache;
        if (memCache == null)
            return null;

        var entriesCollection =
            (IDictionary?)_memoryCacheType.GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(memoryCache);

        if (entriesCollection == null)
            return null;

        var resultList = new List<object>(memCache.Count);
        PropertyInfo? keyProperty = null; // Чтобы инициализировать только один раз, т.к. получения типа - операция дорогая.
        foreach (var entry in entriesCollection)
        {
            if (keyProperty == null)
                keyProperty = entry!.GetType().GetProperty("Key");

            object key = keyProperty!.GetValue(entry)!;
            resultList.Add(key);
        }

        return resultList.ToArray();
    }

    /// <summary>
    /// Удалить данные из кэша по регулярному выражению ключа.
    /// ВНИМАНИЕ!!! Используется рефлексия!
    /// </summary>
    /// <param name="memoryCache">IMemoryCache object</param>
    /// <param name="pattern">регулярное выражение</param>
    public static void RemoveByPattern(this IMemoryCache memoryCache, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return;

        var memCache = memoryCache as MemoryCache;
        if (memCache == null)
            return;

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var allKeys = memoryCache.GetAllKeys();
        if (allKeys == null)
            return;

        var keysToRemove =
            from item in memoryCache.GetAllKeys()
            let stringItem = item.ToString()
            where !string.IsNullOrEmpty(stringItem) && regex.IsMatch(stringItem)
            select item;

        foreach (string key in keysToRemove)
            memoryCache.Remove(key);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    public static void Clear(this IMemoryCache memoryCache) =>
        (memoryCache as MemoryCache)?.Compact(1);
}