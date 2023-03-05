namespace KickStat.Models.GameEvents;

public class GameEventModel
{
    public int Id { get; set; }
    
    public int GameId { get; set; }
    
    public int? PositiveValue { get; set; }

    public int? NegativeValue { get; set; }
    
    public int EventDetailId { get; set; }

    public string LabelPositive { get; set; } = null!;
    
    public string? LabelNegative { get; set; } = null!;
    
    public bool HasNegative { get; set; }

}