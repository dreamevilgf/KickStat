using System.Runtime.CompilerServices;


namespace KickStat;

public static class StringExtensions
{
    /// <summary>
    /// Урезать строку до указанной длины. 
    /// В отличии от Substring не кидает исключение, если строка короче лимита.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Truncate(this string? str, int maxLength) => !string.IsNullOrEmpty(str) && str.Length > maxLength ? str.Substring(0, maxLength) : str;


    /// <summary>
    /// Установить окончание строки заданным набором символов. Если строка уже заканчивается этим набором, ничего не происходит.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="start"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public static string SetStart(this string? str, string start, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        if (str == null)
            return start;

        if (str.StartsWith(start, stringComparison))
            return str;

        return start + str;
    }

    /// <summary>
    /// Установить окончание строки заданным набором символов. Если строка уже заканчивается этим набором, ничего не происходит.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="end"></param>
    /// <param name="stringComparison"></param>
    /// <returns></returns>
    public static string SetEnd(this string? str, string end, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
    {
        if (str == null)
            return end;

        if (str.EndsWith(end, stringComparison))
            return str;

        return str + end;
    }

    /// <summary>
    /// Позволяет использовать String.IsNullOrEmpty() как метод-расширитель на null-значениях
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

    /// <summary>
    /// Позволяет использовать !String.IsNullOrEmpty() как метод-расширитель на null-значениях
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrEmpty(this string? str) => !string.IsNullOrEmpty(str);


    /// <summary>
    /// Позволяет использовать String.IsNullOrWhiteSpace() как метод-расширитель на null-значениях
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    /// <summary>
    /// Позволяет использовать !String.IsNullOrWhiteSpace() как метод-расширитель на null-значениях
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrWhitespace(this string? str) => !string.IsNullOrWhiteSpace(str);


    /// <summary>
    /// Convert hexadecimal string to a byte array
    /// </summary>
    public static byte[]? HexStringToByteArray(this string? hexadecimalString)
    {
        if (hexadecimalString == null)
            return null;

        int numberChars = hexadecimalString.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hexadecimalString.Substring(i, 2), 16);
        return bytes;
    }

    /// <summary>
    /// Translate string from initial format to underscrre case. For example, HelloWorld -> hello_world 
    /// </summary>
    public static string ToUnderscoreCase(this string str) => string.Concat(
        str.Select(
            (charSym, i) => char.IsUpper(charSym)
                ? (i > 0 ? "_" : null) + char.ToLower(charSym)
                : charSym.ToString()
        )
    );

    /// <summary>
    /// Translate string from initial format to slugify case. For example, HelloWorld -> hello-world 
    /// </summary>
    public static string Slugify(this string str) => string.Concat(
        str.Select(
            (charSym, i) => char.IsUpper(charSym)
                ? (i > 0 ? "-" : null) + char.ToLower(charSym)
                : charSym.ToString()
        )
    );
}