using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace KickStat.Extensions;

public static class UrlHelperExtensions
{
    /// <summary>
    /// Комбинировать URL
    /// </summary>
    /// <param name="urlHelper"></param>
    /// <param name="baseUrl">Базовый путь. Рекомендуется не заканчивать /</param>
    /// <param name="paths">Дополнительные пути. Рекомендуется начинать с / или ? или #</param>
    /// <returns></returns>
    public static string Combine(this IUrlHelper urlHelper, string baseUrl, params string[] paths)
    {
        if (paths.Length == 0)
            return baseUrl;

        var sBuilder = new StringBuilder(baseUrl.TrimEnd('/'), baseUrl.Length + paths.Sum(p => p.Length) + paths.Length);
        foreach (string path in paths)
        {
            if (path.StartsWith("?", StringComparison.Ordinal) || path.StartsWith("&", StringComparison.Ordinal) || path.StartsWith("#", StringComparison.Ordinal))
                sBuilder.Append(path);
            else if (path.StartsWith("/", StringComparison.Ordinal))
                sBuilder.Append(path.TrimEnd('/'));
            else
            {
                sBuilder.Append("/");
                sBuilder.Append(path.TrimEnd('/'));
            }
        }

        // Восстанавливаем только последний слеш, если он есть
        if (paths[paths.Length - 1].EndsWith("/", StringComparison.Ordinal))
            sBuilder.Append("/");

        return sBuilder.ToString();
    }

    public static string Encode(this IUrlHelper urlHelper, string url)
    {
        return WebUtility.UrlEncode(url);
    }

    public static string Decode(this IUrlHelper urlHelper, string url)
    {
        return WebUtility.UrlDecode(url);
    }
}