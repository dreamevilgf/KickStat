using System.Net.Http.Json;
using KickStat.App.Framework.Extensions;
using KickStat.Models;

namespace KickStat.App.Framework.KickStatApi;

public class KickStatAuthApiClient
{
    private readonly HttpClient _http;
    private readonly TokenService _tokenService;

    public KickStatAuthApiClient(HttpClient http, TokenService tokenService)
    {
        _http = http;
        _tokenService = tokenService;
    }

    public async Task<JwtTokenResponse> Token(string email, string password)
    {
        var response = await _http.PostAsJsonAsync($"api/auth/token", new LoginRequest() { Login = email, Password = password });

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<JwtTokenResponse>();

        await _tokenService.Set(result!);

        return result!;
    }
}