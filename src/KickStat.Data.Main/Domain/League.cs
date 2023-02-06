using System.ComponentModel.DataAnnotations.Schema;

namespace KickStat.Data.Domain;

[Table("leagues")]
public class League
{
    [Column("id")] public int Id { get; set; }

    [Column("title")] public string Title { get; set; } = null!;
}