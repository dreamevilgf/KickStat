using System.Runtime.CompilerServices;


namespace KickStat;

public static class CollectionExtensions
{
    /// <summary>
    /// Проверка на пустоту коллекции. В случае null-коллекции вернет true.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this ICollection<T>? collection) => collection == null || collection.Count == 0;

    /// <summary>
    /// Проверка на то, что коллекция непустая. В случае null-коллекции вернет false.  
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrEmpty<T>(this ICollection<T>? collection) => collection?.Count > 0;
}