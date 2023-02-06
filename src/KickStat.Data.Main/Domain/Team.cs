using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KickStat.Data.Domain;

[Table("teams")]
public class Team
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    [MaxLength(255)]
    [Required]
    public string Title { get; set; } = null!;

    public virtual List<Player> Players { get; set; } = new List<Player>();
}