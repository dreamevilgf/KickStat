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
public class HandleAndLogErrorAttribute : ExceptionFilterAttribute
{
    /// <summary>
    /// Требуется ли логирование исключения (по умолчанию true)
    /// </summary>
    public bool IsNeedLog { get; set; }

    public HandleAndLogErrorAttribute()
    {
        IsNeedLog = true;
    }

    public override void OnException(ExceptionContext exceptionContext)
    {
        if (exceptionContext == null)
            throw new ArgumentNullException(nameof(exceptionContext));

        if (exceptionContext.ExceptionHandled)
        {
            base.OnException(exceptionContext);
            return;
        }

        int eventId = exceptionContext.Exception.Data.SafeGet("errorCode", ErrorCodes.UnhandledException);
        if (!(exceptionContext.Exception is IAlreadyLoggedException) && this.IsNeedLog)
        {
            var logger = exceptionContext.HttpContext.RequestServices.GetRequiredService<ILogger<AppErrorLogger>>();
            logger.LogError(new EventId(eventId, eventId.ToString()), exceptionContext.Exception, exceptionContext.Exception.Message);
        }


        if (exceptionContext.HttpContext.Request.IsAjax())
        {
            switch (exceptionContext.Exception)
            {
                // Переопределяем результат в JSON
                case ModelStateApiException problemDetailsException:
                    exceptionContext.Result = new JsonResult(problemDetailsException.ProblemDetails);
                    exceptionContext.HttpContext.Response.StatusCode = problemDetailsException.ProblemDetails.Status ?? StatusCodes.Status500InternalServerError;
                    break;
                case ApiException apiException:
                {
                    var errorInfo = apiException.ProblemDetails;
#if DEBUG
                    if (errorInfo.Status == StatusCodes.Status500InternalServerError || errorInfo.Status == ErrorCodes.UnhandledException)
                        errorInfo.Extensions["stackTrace"] = apiException.StackTrace;
#endif
                    exceptionContext.Result = new JsonResult(errorInfo);
                    exceptionContext.HttpContext.Response.StatusCode = apiException.ProblemDetails.Status ?? StatusCodes.Status500InternalServerError;
                    break;
                }
                default:
                {
                    var errorInfo = new ProblemDetails
                    {
                        Title = exceptionContext.Exception.Message,
                        Status = eventId,
                        Type = exceptionContext.Exception.GetType().Name
                    };

#if DEBUG
                    if (errorInfo.Status == StatusCodes.Status500InternalServerError || errorInfo.Status == ErrorCodes.UnhandledException)
                        errorInfo.Extensions["stackTrace"] = exceptionContext.Exception.StackTrace;
#endif

                    exceptionContext.Result = new JsonResult(errorInfo);
                    exceptionContext.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
                }
            }

            //exceptionContext.ExceptionHandled = true;
            return;
        }

        // Если не AJAX, то даем отработать штатному middleware. В AppLogger это уже не запишется.
        throw new AlreadyLoggedException(exceptionContext.Exception.Message, exceptionContext.Exception);
    }
}