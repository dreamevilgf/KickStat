using System.Net;
using KickStat.Client.Api.Configuration;

namespace KickStat.Client.Api.Framework;

internal class ApiHttpClientFactory
{
    public static HttpClient Create(KickStatApiConfig serviceConfig, IHttpClientFactory? httpClientFactory = null)
    {
        if (serviceConfig == null)
            throw new NullReferenceException("Не задана конфигурация сервиса для Viju.Api.Client");

        string adjustedServiceUrl = serviceConfig.BaseUrl;
        if (!adjustedServiceUrl.EndsWith("/", StringComparison.Ordinal))
            adjustedServiceUrl += "/"; // Это нужно для HttpClient-а, иначе BaseAddress будет работать некорректно

        HttpClient httpClient;
        if (httpClientFactory != null)
        {
            httpClient = httpClientFactory.CreateClient(ServiceRegistrationExtensions.HTTP_CLIENT_NAME);
            httpClient.BaseAddress = new Uri(adjustedServiceUrl);
        }
        else
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All, AllowAutoRedirect = false };
            httpClient = new HttpClient(handler) { BaseAddress = new Uri(adjustedServiceUrl) };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
        }

        return httpClient;
    }


    public static (bool, HttpClient?) TryCreate(KickStatApiConfig serviceConfig, IHttpClientFactory? httpClientFactory = null)
    {
        try
        {
            HttpClient httpClient = Create(serviceConfig, httpClientFactory);
            return (true, httpClient);
        }
        catch
        {
            return (false, null);
        }
    }
}