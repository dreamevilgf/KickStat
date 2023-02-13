namespace KickStat.Models.Games;

public class GameListRequest
{
    public GameFilterModel Filter { get; set; } = new();
    
    public GameSortModel Sort { get; set; } = new();
}