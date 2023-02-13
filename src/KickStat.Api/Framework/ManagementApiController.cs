using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MultiVod.UI.SiteApi.Framework.Config;

namespace KickStat.UI.SiteApi.Framework;

[ApiController]
public abstract class ManagementApiController : ControllerBase, IAsyncActionFilter
{
    protected ILogger Logger => _logger ??= (ILogger)HttpContext.RequestServices.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType())); // get current class logger
    private ILogger? _logger;

    protected IMemoryCache MemoryCache => _memoryCache ??= HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
    private IMemoryCache? _memoryCache;


    [NonAction]
    public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //await Task.Delay(2000);
        await next();
    }
}