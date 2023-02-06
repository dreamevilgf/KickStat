using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace KickStat;

/// <summary>
/// Класс для исключений для возврата значений из API. Исключения этого типа маркируются как уже залогированные.
/// </summary>
public class ApiException : ApplicationException, IAlreadyLoggedException
{
    /// <summary>
    /// Объект, который в дальнейшем будет полностью сериализован в ответ
    /// </summary>
    public ProblemDetails ProblemDetails { get; }


    public ApiException(string message, int statusCode = StatusCodes.Status500InternalServerError)
        : this(message, message, statusCode)
    {
    }

    public ApiException(string message, string? detail, int statusCode = StatusCodes.Status500InternalServerError)
        : this(new ProblemDetails { Title = message, Detail = detail, Status = statusCode })
    {
    }


    // Вся реализация здесь
    public ApiException(ProblemDetails errorData, Exception? inner = null) : base(errorData.Title, inner)
    {
        this.ProblemDetails = errorData;
        if (string.IsNullOrEmpty(this.ProblemDetails.Type))
            this.ProblemDetails.Type = this.GetType().Name;
    }
}