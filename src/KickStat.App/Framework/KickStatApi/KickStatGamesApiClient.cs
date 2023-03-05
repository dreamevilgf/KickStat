using System.Net.Http.Json;
using KickStat.App.Framework.Enums;
using KickStat.App.Framework.Extensions;
using KickStat.Models;
using KickStat.Models.GameEvents;
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

    public async Task<GameEditModel> Save(GameEditModel editingItem)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.PostAsJsonWithAuthAsync("/api/games/save", editingItem, tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<GameEditModel>();

        return result!;
    }

    public async Task<GameEditModel> Get(int id)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.GetWithAuthAsync<GameEditModel>($"api/games/{id}", tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<GameEditModel>();

        return result!;
    }
    
    public async Task<GameModel> GetStats(int id)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));


        var response = await _http.GetWithAuthAsync<GameModel>($"api/games/{id}/stats", tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<GameModel>();

        return result!;
    }

    public async Task<List<EventDetailModel>> GetEventDetails(EventDetailType type)
    {
        var tokenInfo = await _tokenService.Get();
        if (tokenInfo == null)
            throw new ArgumentNullException(nameof(tokenInfo));
        
        var response = await _http.GetWithAuthAsync<List<EventDetailModel>>($"api/event-details?type={type:G}", tokenInfo.AccessToken);

        await response.IfIsNotSuccessThrowException();

        var result = await response.Content.ReadFromJsonAsync<List<EventDetailModel>>();

        return result!;
    }
}