
using System.Net.Http.Headers;
using System.Net.Http.Json;
using KickStat.Models;

namespace KickStat.App.Framework.Extensions;

public static class HttpClientExtensions
{
    public static async Task IfIsNotSuccessThrowException(this HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var ex = new Exception("Alaram");
            var error = await response.Content.ReadFromJsonAsync<ErrorModel>();
            if (error != null)
                ex.Data.Add("error", error.Detail);

            throw ex;
        }
    }


    public static async Task<HttpResponseMessage> PostAsJsonWithAuthAsync<T>(this HttpClient client, string url, T obj, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.PostAsJsonAsync(url, obj);

        return response;
    }

    public static async Task<HttpResponseMessage> GetWithAuthAsync<T>(this HttpClient client, string url, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync(url);

        return response;
    }

    public static async Task<HttpResponseMessage> DeleteWithAuthAsync<T>(this HttpClient client, string url, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.DeleteAsync(url);

        return response;
    }
}