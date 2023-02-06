using System.Text.Json.Serialization;

namespace KickStat.Client.Api.DTO;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; set; } = null!;

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
}