using MultiVod.UI.SiteApi.DTO;

namespace KickStat.Client.Api.DTO.Players;

public class PlayerFilterModel : FilterBaseModel
{
    public string? Query { get; set; }
    
}