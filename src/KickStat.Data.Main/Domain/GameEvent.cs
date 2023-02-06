using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootStat.Data.Entities.Enums;
using KickStat.Data.Domain;

namespace KickStat.Data.Domain;

[Table("game_events")]
public class GameEvent
{
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    [Required]
    [Column("first_player_id")]
    public int FirstPlayerId { get; set; }

    [Column("second_player_id")]
    public int? SecondPlayerId { get; set; }

    [Column("time")]
    public int Time { get; set; }

    [Required]
    [Column("event")]
    public EventType EventType { get; set; }

    [Required]
    [Column("position_first_player")]
    public PositionType PositionFirstPlayer { get; set; }
    
    [Column("position_second_player")]
    public PositionType PositionSecondPlayer { get; set; }

    [ForeignKey(nameof(FirstPlayerId))] 
    public Player FirstPlayer { get; set; } = null!;

    [ForeignKey(nameof(SecondPlayerId))] 
    public Player? SecondPlayer { get; set; }

    [ForeignKey(nameof(GameId))] public Game Game { get; set; } = null!;

    [Column("coords_first_player")]
    public int[]? CoordsFirstPlayer { get; set; }
    
    [Column("coords_second_player")]
    public int[]? CoordsSecondPlayer { get; set; }
}