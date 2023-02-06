using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace KickStat.Extensions;

public static class LinkGeneratorExtensions
{
    public static string Content(this LinkGenerator linkGenerator, HttpContext httpContext, string url)
    {
        if (url.StartsWith("~/"))
            url = httpContext.Request.PathBase.Value + url.Remove(0, 1); // убираем тильду ~

        return url;
    }
}