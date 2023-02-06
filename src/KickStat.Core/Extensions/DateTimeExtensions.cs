using System.Runtime.CompilerServices;


namespace KickStat;

/// <summary>
/// Класс-расширитель типа DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Преобразование даты в формат строки SQLServer ('0000-00-00 00:00:00')
    /// </summary>
    /// <param name="dateTime">Тип расширяемого объекта</param>
    /// <returns>Дата в формате строки SQLServer ('0000-00-00 00:00:00')</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSqlServerString(this DateTime dateTime) => dateTime.ToString(@"\'yyyy\-MM\-dd HH\:mm\:ss\'");


    /// <summary>
    /// Находится ли дата в заданном диапазоне. Проверяет больше-равно начального и меньше ли конечного времени.
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="startDate">Начальное время</param>
    /// <param name="endDate">Конечное время</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBetween(this DateTime dt, DateTime startDate, DateTime endDate) => dt >= startDate && dt < endDate;


    /// <summary>
    /// Converts a unix time string to a DateTime object.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="unixTimeSeconds">The unix time.</param>
    /// <returns>The DateTime object.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime FromUnixTime(this DateTime dateTime, double unixTimeSeconds) => DateTime.UnixEpoch.AddSeconds(unixTimeSeconds);


    /// <summary>
    /// Converts a DateTime object to unix time (in seconds).
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The unix date time in seconds from Unix epoch.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToUnixTime(this DateTime dateTime) => (long)(dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;

    /// <summary>
    /// Converts a DateTimeOffset object to unix time (in seconds).
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The unix date time in seconds from Unix epoch.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToUnixTime(this DateTimeOffset dateTime) => (long)(dateTime.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;
}