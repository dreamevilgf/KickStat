using System.Collections.Concurrent;

using Microsoft.Extensions.Caching.Memory;


namespace KickStat;

/// <summary>
/// Extensions
/// </summary>
public static partial class MemoryCacheExtensions
{
    public static T Get<T>(this IMemoryCache memoryCache, string key, Func<T> acquire) => Get(memoryCache, key, TimeSpan.FromSeconds(60), acquire);

    /// <summary>
    /// Взять элемент из кэша и если его нет в кэше, выполнить функцию по его получению и положить в кэш
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="memoryCache">Объект кэша</param>
    /// <param name="key">Ключ</param>
    /// <param name="cacheTime">Время, на которое полученный элемент кладется в кэш</param>
    /// <param name="acquire">Делегат для получения элемента</param>
    /// <returns></returns>
    public static T Get<T>(this IMemoryCache memoryCache, string key, TimeSpan cacheTime, Func<T> acquire)
    {
        if (memoryCache.IsSet(key))
            return memoryCache.Get<T>(key);

        var result = acquire();
        memoryCache.Set(key, result, cacheTime);

        return result;
    }


    public static async Task<T> Get<T>(this IMemoryCache memoryCache, string key, Func<Task<T>> acquire) => await Get(memoryCache, key, TimeSpan.FromSeconds(60), acquire);

    /// <summary>
    /// Взять элемент из кэша и если его нет в кэше, выполнить функцию по его получению и положить в кэш (асинхронная версия)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="memoryCache">Объект кэша</param>
    /// <param name="key">Ключ</param>
    /// <param name="cacheTime">Время, на которое полученный элемент кладется в кэш</param>
    /// <param name="acquire">Делегат для получения элемента</param>
    /// <returns></returns>
    public static async Task<T> Get<T>(this IMemoryCache memoryCache, string key, TimeSpan cacheTime, Func<Task<T>> acquire)
    {
        if (memoryCache.IsSet(key))
            return memoryCache.Get<T>(key);

        var result = await acquire.Invoke();

        memoryCache.Set(key, result, cacheTime);
        return result;
    }


    private static readonly ConcurrentDictionary<string, object> _lockObjects = new(); // one object for each key

    /// <summary>
    /// Получение объекта с внесением в кэш всего одним потоком из домена. Остальные ожидающие потоки получат данные уже из кэша.
    /// Так исключается ситуация, когда данные вынимаются, кэша еще нет, а другой поток обращается к тем же данным.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="cacheTime"></param>
    /// <param name="acquire"></param>
    /// <returns></returns>
    public static T GetWithExclusiveLock<T>(this IMemoryCache cache, string key, TimeSpan cacheTime, Func<T> acquire)
    {
        if (cache.IsSet(key))
            return cache.Get<T>(key);

        T result;
        object lockObj = _lockObjects.GetOrAdd(key, new object());
        lock (lockObj)
        {
            // Check again. Maybe some other thread already got it
            if (cache.IsSet(key))
                return cache.Get<T>(key);


            result = acquire.Invoke();
            cache.Set(key, result, cacheTime);
        }

        return result;
    }


    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _lockObjectsAsync = new(); // one semaphore for each key

    /// <summary>
    /// Получение объекта с внесением в кэш всего одним потоком из домена. Остальные ожидающие потоки получат данные уже из кэша.
    /// Так исключается ситуация, когда данные вынимаются, кэша еще нет, а другой поток обращается к тем же данным.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="cacheTime"></param>
    /// <param name="acquire"></param>
    /// <returns></returns>
    public static async Task<T> GetWithExclusiveLock<T>(this IMemoryCache cache, string key, TimeSpan cacheTime, Func<Task<T>> acquire)
    {
        if (cache.IsSet(key))
            return cache.Get<T>(key);

        T result;
        SemaphoreSlim semaphoreSlim = _lockObjectsAsync.GetOrAdd(key, new SemaphoreSlim(1, 1));

        await semaphoreSlim.WaitAsync();
        try
        {
            // Check again. Maybe some other thread already got it
            if (cache.IsSet(key))
                return cache.Get<T>(key);

            result = await acquire.Invoke();
            cache.Set(key, result, cacheTime);
        }
        finally
        {
            //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
            //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
            semaphoreSlim.Release();
        }

        return result;
    }
}