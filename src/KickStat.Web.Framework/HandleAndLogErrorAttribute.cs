using FarPlan.Logging;
using KickStat.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KickStat;

/// <summary>
/// Атрибут, используемый для обработки и логирования исключений
/// </summary>
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ApiException apiException)
        {
            context.Result = new ObjectResult(apiException.Value)
            {
                StatusCode = apiException.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}