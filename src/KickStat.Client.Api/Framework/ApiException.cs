using System.Text.Json;
using KickStat.Client.Api.DTO;
using Microsoft.AspNetCore.Http;

namespace KickStat.Client.Api.Framework;

public class ApiException : ApplicationException
{
    public int StatusCode { get; set; }
    public string? Content { get; set; }

    public ApiException()
    {
    }

    public ApiException(string message, int statusCode = StatusCodes.Status500InternalServerError, string? content = null) : base(message)
    {
        this.StatusCode = statusCode;
        this.Content = content;
    }

    public ApiException(string message, Exception inner) : base(message, inner)
    {
    }

    public ErrorResponse? GetErrorResponseFromContent()
    {
        if (string.IsNullOrEmpty(Content))
            return null;

        try
        {
            return JsonSerializer.Deserialize<ErrorResponse>(this.Content);
        }
        catch
        {
            return null;
        }
    }
}