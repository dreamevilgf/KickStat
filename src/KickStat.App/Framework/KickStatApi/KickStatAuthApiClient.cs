using System.Net.Http.Json;
using KickStat.App.Framework.Extensions;
using KickStat.Models;

namespace KickStat.App.Framework.KickStatApi;

public class KickStatAuthApiClient
{
    private readonly HttpClient _http;
    private readonly TokenService _tokenService;
    private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

    public KickStatAuthApiClient(HttpClient http, TokenService tokenService, CustomAuthenticationStateProvider authenticationStateProvider)
    {
        _http = http;
        _tokenService = tokenService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<JwtTokenResponse> Token(string email, string password)
    {
        var response = await _http.PostAsJsonAsync($"api/auth/token", new LoginRequest() { Login = email, Password = password });

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<JwtTokenResponse>();

        await _tokenService.Set(result!);
        _authenticationStateProvider.StateChanged();

        return result!;
    }
    
    public async Task LogoutUser()
    {
        await _tokenService.Remove();
        _authenticationStateProvider.StateChanged();
    }
}