using Blazored.LocalStorage;
using KickStat.Models;

namespace KickStat.App.Framework;

public class TokenService
{
    private readonly ILocalStorageService _localStorageService;

    public TokenService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task Set(JwtTokenResponse token)
    {
        await _localStorageService.SetItemAsync("token", token);
    }

    public async Task<JwtTokenResponse?> Get()
    {
        if (!await _localStorageService.ContainKeyAsync("token"))
            return null;

        return await _localStorageService.GetItemAsync<JwtTokenResponse>("token");


    }

    public async Task Remove()
    {
        await _localStorageService.RemoveItemAsync("token");
    }
}