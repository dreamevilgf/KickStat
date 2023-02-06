using System.Net;
using System.Net.Http.Headers;
using KickStat.Client.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace KickStat.Client.Api;

public static class ServiceRegistrationExtensions
{
    internal const string HTTP_CLIENT_NAME = "KickStartApiHttpClient";

    public static IServiceCollection AddKickStatAPI(this IServiceCollection services,
        IConfiguration? configuration = null, string configurationSection = "kickStatApi")
    {
        if (configuration != null && !string.IsNullOrEmpty(configurationSection))
            services.Configure<KickStatApiConfig>(configuration.GetSection(configurationSection));

        services.AddHttpClient(HTTP_CLIENT_NAME, (_, client) =>
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        })
            .ConfigureHttpMessageHandlerBuilder(config => config.PrimaryHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.All, AllowAutoRedirect = false })
            .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

        services.AddScoped<AuthApi>();

        return services;
    }
}