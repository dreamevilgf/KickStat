using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KickStat.Data.Domain;

namespace KickStat.Data.Domain;

[Table("game_events")]
public class GameEvent
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("game_id")]
    public int GameId { get; set; }
    
    [Column("event_detail_id")]
    public int EventDetailId { get; set; }

    [Column("value")]
    public int Value { get; set; }
    
    [ForeignKey(nameof(GameId))]
    public Game? Game { get; set; }
}