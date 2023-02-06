using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootStat.Data.Entities.Enums;
using KickStat.Data.Domain;
using KickStat.Data.Domain.Identity;

namespace KickStat.Data.Domain;

[Table("games")]
public class Game
{
    [Column("id")]
    public int Id { get; set; }

    [Column("opposing_team")]
    [Required]
    public string OpposingTeam { get; set; } = null!;
    
    [Column("date")]
    public DateTime Date { get; set; }
    

    [Column("meta", TypeName = "jsonb")]
    public GameMeta Meta { get; set; } = new();

    public IList<GameEvent> Events { get; set; } = new List<GameEvent>();

    [Required]
    public Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public KickStatUser? User { get; set; }
}


public class GameMeta
{
    public int? TotalMissedGoals { get; set; }
    
    public int? Playtime { get; set; }

    public int? MatchDuration { get; set; }

    public string? Competition { get; set; }
    
    public bool IsMainTeam { get; set; }
    
}



