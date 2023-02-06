using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using System.Text.Json;
using KickStat.Client.Api.Configuration;
using KickStat.Client.Api.Framework;
using KickStat.Models;

namespace KickStat.Client.Api;

public class AuthApi
{

    private readonly IHttpClientFactory? _httpClientFactory;
    private readonly KickStatApiConfig _kickStatApiConfig;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);


    public AuthApi(IOptions<KickStatApiConfig> config, IHttpClientFactory httpClientFactory) : this(
        config.Value, httpClientFactory)
    {
    }

    public AuthApi(KickStatApiConfig kickStatApiConfig, IHttpClientFactory? httpClientFactory = null)
    {
        _kickStatApiConfig = kickStatApiConfig;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<JwtTokenResponse> Token(string email, string password)
    {
        var requestUrl = "/api/auth/token";

        var httpClient = ApiHttpClientFactory.Create(_kickStatApiConfig, _httpClientFactory);

        var response = await httpClient.PostAsJsonAsync(requestUrl, new LoginRequest()
        {
            Login = email,
            Password = password
        });


        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JwtTokenResponse>(content)!;

        return result;

    }
}