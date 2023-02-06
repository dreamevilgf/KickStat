using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace KickStat;

/// <summary>
/// Класс для исключений валидации модели для возврата значений из API. Исключения этого типа маркируются как уже залогированные.
/// </summary>
public class ModelStateApiException : ApiException
{
    public ModelStateApiException(ModelStateDictionary modelState, string? title = null, int statusCode = StatusCodes.Status400BadRequest) :
        base(new ValidationProblemDetails(modelState))
    {
        if (!string.IsNullOrEmpty(title))
            this.ProblemDetails.Title = title;

        this.ProblemDetails.Status = statusCode;
    }
}