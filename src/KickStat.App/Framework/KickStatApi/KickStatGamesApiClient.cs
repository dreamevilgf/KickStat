using System.Net.Http.Json;
using KickStat.App.Framework.Extensions;
using KickStat.Models;
using KickStat.Models.Games;

namespace KickStat.App.Framework.KickStatApi;

public class KickStatGamesApiClient
{
    private readonly HttpClient _http;
    private readonly TokenService _tokenService;

    public KickStatGamesApiClient(HttpClient http, TokenService tokenService)
    {
        _http = http;
        _tokenService = tokenService;
    }

    public async Task<PagedResult<GameListModel>> List(GameListRequest filter)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.PostAsJsonWithAuthAsync("/api/games", filter, tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<PagedResult<GameListModel>>();

        return result!;
    }

    public async Task<GameModel> Save(GameModel editingItem)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.PostAsJsonWithAuthAsync("/api/games/save", editingItem, tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<GameModel>();

        return result!;
    }

    public async Task<GameModel> Get(int id)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.GetWithAuthAsync<GameModel>($"api/games/{id}", tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<GameModel>();

        return result!;
    }
}