namespace KickStat.Client.Api.DTO.Players;

public class PlayerSortModel
{
    public PlayerSortOptions OrderBy { get; set; }

    public bool IsAscending { get; set; }
}