namespace KickStat.Models.GameEvents;

public class GameEventModel
{
    public int Id { get; set; }
    
    public int GameId { get; set; }
    
    public int Value { get; set; }
    
    public EventDetailModel EventDetail { get; set; }
    
}