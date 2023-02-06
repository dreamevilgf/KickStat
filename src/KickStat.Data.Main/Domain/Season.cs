using KickStat.Data.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace KickStat.Data.Domain;

[Table("seasons")]
public class Season
{
    [Column("id")] public int Id { get; set; }

    [Column("title")] public string Title { get; set; } = null!;
    [Column("league_id")] public int LeagueId { get; set; }

    [ForeignKey(nameof(LeagueId))] public League? League { get; set; }
}