using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KickStat.Data.Domain.Identity;

namespace KickStat.Data.Domain;

[Table("games")]
public class Game
{
    [Column("id")] public int Id { get; set; }

    [Column("opposing_team")]
    [Required] 
    public string OpposingTeam { get; set; } = null!;

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }
    
    [Column("meta", TypeName = "jsonb")]
    public GameMeta Meta { get; set; } = new();

    [Column("player_id")]
    public int PlayerId { get; set; }
    
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(PlayerId))] public Player? Player { get; set; }

    public List<GameEvent> Events { get; set; } = new ();

    [Required] public Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))] public KickStatUser? User { get; set; }
}

public class GameMeta
{
    public int? TotalMissedGoals { get; set; }

    public int? Playtime { get; set; }

    public int MatchDuration { get; set; }

    public string? Competition { get; set; }

    public bool IsMain { get; set; }
}