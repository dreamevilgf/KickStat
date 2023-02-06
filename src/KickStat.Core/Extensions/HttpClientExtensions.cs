using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace KickStat;

public static class HttpClientExtensions
{
    #region Заголовки

    public static string? Get(this HttpHeaders headers, string name)
    {
        if (headers.TryGetValues(name, out var values))
            return string.Join(",", values);

        return null;
    }

    public static void Set(this HttpHeaders headers, string name, string value, bool needValidation = false)
    {
        if (headers.Contains(name))
            headers.Remove(name);

        if (needValidation)
            headers.Add(name, value);
        else
            headers.TryAddWithoutValidation(name, value);
    }

    #endregion

    #region Методы

    public static async Task<TResult?> GetAsync<TResult>(this HttpClient httpClient, string url)
    {
        var response = await httpClient.GetAsync(url);
        response.ThrowIfNonSuccess();

        TResult? result;
        try
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            result = await JsonSerializer.DeserializeAsync<TResult>(responseStream);
        }
        catch (Exception ex)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Deserialization error. URL: {response.RequestMessage?.RequestUri}, Content: {content}", ex);
        }

        return result;
    }


    public static Task<HttpResponseMessage> DeleteAsync(this HttpClient httpClient, string requestUri, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = content };
        return httpClient.SendAsync(request);
    }


    public static void ThrowIfNonSuccess(this HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.IsSuccessStatusCode)
            return;

        string jsonResponse = httpResponseMessage.Content.ReadAsStringAsync().Result;
        string message = $"Response error: {httpResponseMessage.StatusCode:D} {httpResponseMessage.ReasonPhrase}. URL: {httpResponseMessage.RequestMessage?.RequestUri}, Content: {jsonResponse}";
        throw new HttpRequestException(message);
    }

    #endregion
}

public class JsonContent : StringContent
{
    public JsonContent(object content, string mediaType = "application/json") : this(content, Encoding.UTF8, mediaType)
    {
    }

    public JsonContent(object content, Encoding encoding, string mediaType) : base(JsonSerializer.Serialize(content), encoding, mediaType)
    {
    }
}