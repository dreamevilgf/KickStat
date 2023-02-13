using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace KickStat;


public class ApiException : Exception
{
    public ApiException(object value, int statusCode = 500) =>
        (StatusCode, Value) = (statusCode, value);

    public int StatusCode { get; }

    public object? Value { get; }
}