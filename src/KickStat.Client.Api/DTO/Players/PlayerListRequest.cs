namespace KickStat.Client.Api.DTO.Players;

public class PlayerListRequest
{
    public PlayerFilterModel Filter { get; set; } = new();
    
    public PlayerSortModel Sort { get; set; } = new();
}