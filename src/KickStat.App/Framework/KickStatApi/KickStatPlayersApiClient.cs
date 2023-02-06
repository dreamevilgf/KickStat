using System.Net.Http.Json;
using KickStat.App.Framework.Extensions;
using KickStat.Models;
using KickStat.Models.Players;

namespace KickStat.App.Framework.KickStatApi;

public class KickStatPlayersApiClient
{
    private readonly HttpClient _http;
    private readonly TokenService _tokenService;

    public KickStatPlayersApiClient(HttpClient http, TokenService tokenService)
    {
        _http = http;
        _tokenService = tokenService;
    }

    public async Task<PagedResult<PlayerModel>> List(PlayerListRequest filter)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.PostAsJsonWithAuthAsync("/api/players", filter, tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<PagedResult<PlayerModel>>();

        return result!;
    }

    public async Task<PlayerModel> Save(PlayerModel editingItem)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.PostAsJsonWithAuthAsync("/api/players/save", editingItem, tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<PlayerModel>();

        return result!;
    }

    public async Task<PlayerModel> Get(int id)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.GetWithAuthAsync<PlayerModel>($"/players/edit/{id}", tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<PlayerModel>();

        return result!;
    }
}