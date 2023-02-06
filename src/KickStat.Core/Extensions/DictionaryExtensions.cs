using System.Collections;
using System.Runtime.CompilerServices;


namespace KickStat;

public static class DictionaryExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : defaultValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue? SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull =>
        dictionary.TryGetValue(key, out var value) ? value : default;


    public static TValue SafeGet<TValue>(this IDictionary dictionary, object key, TValue defaultValue)
    {
        TValue value = defaultValue;

        if (dictionary.Contains(key))
        {
            object? valObj = dictionary[key];
            if (valObj is TValue obj)
                value = obj;
        }

        return value;
    }

    public static TValue? SafeGet<TValue>(this IDictionary dictionary, object key)
    {
        TValue? value = default;

        if (dictionary.Contains(key))
        {
            object? valObj = dictionary[key];
            if (valObj is TValue obj)
                value = obj;
        }

        return value;
    }
}