namespace KickStat.Caching;

/// <summary>
/// Интерфейс кэша
/// </summary>
public interface ICache
{
    /// <summary>
    /// Получить данные из кэша
    /// </summary>
    /// <typeparam name="T">Тип</typeparam>
    /// <param name="key">Ключ значения</param>
    /// <returns>Значение, ассоциированное с данным ключом кэша.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Добавить ключ и его данные в кэш
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="data">Значение</param>
    /// <param name="cacheTime">Cache time</param>
    void Set(string key, object? data, TimeSpan cacheTime);

    /// <summary>
    /// Добавить ключ и его данные в кэш
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="data">Значение</param>
    /// <param name="cacheTimeSeconds">Время кэша в секундах</param>
    void Set(string key, object? data, int cacheTimeSeconds = 120);

    /// <summary>
    /// Проверить находятся ли данные с переданным ключом в кэше
    /// </summary>
    /// <param name="key">Ключ</param>
    bool IsSet(string key);

    /// <summary>
    /// Удалить данные из кэша по ключу
    /// </summary>
    /// <param name="key">/Ключ</param>
    void Remove(string key);

    /// <summary>
    /// Удалить данные из кэша по регулярному выражению ключа
    /// </summary>
    /// <param name="pattern">регулярное выражение</param>
    void RemoveByPattern(string pattern);

    /// <summary>
    /// Очистка всех данных из кэша
    /// </summary>
    void Clear();

    object? this[string key] { get; set; }
}