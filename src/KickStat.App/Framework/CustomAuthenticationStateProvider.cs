using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace KickStat.App.Framework;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;

    public CustomAuthenticationStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public void StateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.Get();
        var identity = token == null || string.IsNullOrEmpty(token.AccessToken) || token.ExpiresIn < DateTime.UtcNow
            ? new ClaimsIdentity()
            : new ClaimsIdentity(ParseClaimsFromJwt(token.AccessToken), "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}