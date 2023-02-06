using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace KickStat.Caching;

/// <summary>
/// Represents a NopStaticCache
/// </summary>
public class PerRequestCacheManager : ICache
{
    private readonly HttpContext _context;


    public PerRequestCacheManager(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext == null)
            throw new ArgumentNullException(nameof(httpContextAccessor), "Вы не можете использовать кэш запроса вне Web-приложения");

        _context = httpContextAccessor.HttpContext;
    }

    /// <summary>
    /// Creates a new instance of the NopRequestCache class
    /// </summary>
    protected IDictionary<object, object?> GetItems() => _context.Items;


    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    public T? Get<T>(string key)
    {
        var item = GetItems()[key];
        if (item == null)
            return default;

        return (T)item;
    }

    /// <summary>
    /// Добавляет ключ и его значение в кэш
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="data">Data</param>
    /// <param name="cacheTime">Cache time</param>
    public void Set(string key, object? data, TimeSpan cacheTime)
    {
        var items = GetItems();

        if (data != null)
            items.Add(key, data);
    }

    /// <summary>
    /// Adds the specified key and object to the cache.
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="data">Data</param>
    /// <param name="cacheTimeSeconds">Cache time</param>
    public void Set(string key, object? data, int cacheTimeSeconds = 120)
    {
        Set(key, data, TimeSpan.Zero);
    }

    /// <summary>
    /// Gets a value indicating whether the value associated with the specified key is cached
    /// </summary>
    /// <param name="key">key</param>
    /// <returns>Result</returns>
    public bool IsSet(string key)
    {
        var items = GetItems();
        return items[key] != null;
    }

    /// <summary>
    /// Removes the value with the specified key from the cache
    /// </summary>
    /// <param name="key">/key</param>
    public void Remove(string key)
    {
        var items = GetItems();
        items.Remove(key);
    }

    /// <summary>
    /// Removes items by pattern
    /// </summary>
    /// <param name="pattern">pattern</param>
    public void RemoveByPattern(string pattern)
    {
        var items = GetItems();

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var keysToRemove = new List<string>();
        foreach (var item in items)
        {
            string? itemKey = item.Key.ToString();
            if (itemKey != null && regex.IsMatch(itemKey))
                keysToRemove.Add(itemKey);
        }

        foreach (string key in keysToRemove)
            items.Remove(key);
    }

    /// <summary>
    /// Clear all cache data
    /// </summary>
    public void Clear()
    {
        var items = GetItems();

        var keysToRemove = new List<string>();
        foreach (var item in items)
        {
            string? itemKey = item.Key.ToString();
            if (!string.IsNullOrEmpty(itemKey))
                keysToRemove.Add(itemKey);
        }

        foreach (var key in keysToRemove)
            items.Remove(key);
    }

    public object? this[string key]
    {
        get => Get<object>(key);
        set => Set(key, value);
    }
}