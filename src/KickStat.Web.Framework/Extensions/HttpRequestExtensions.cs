using System.Net;
using Microsoft.AspNetCore.Http;

namespace KickStat.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsAjax(this HttpRequest request)
    {
        string accept = request.Headers["Accept"];
        return accept?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true
               ||
               string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }


    public static bool IsLocal(this HttpRequest request)
    {
        var connection = request.HttpContext.Connection;
        if (connection.RemoteIpAddress != null)
        {
            if (connection.LocalIpAddress != null)
                return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);

            return IPAddress.IsLoopback(connection.RemoteIpAddress);
        }

        // for in memory TestServer or when dealing with default connection info
        if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            return true;

        return false;
    }
}