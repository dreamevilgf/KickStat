using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KickStat.Data.Domain.Identity;

namespace KickStat.Data.Domain;

[Table("players")]
public class Player
{
    [Column("id")]
    public int Id { get; set; }

    [Column("full_name")]
    [Required] public string FullName { get; set; } = null!;

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("birth_year")]
    public int? BirthYear { get; set; }

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    [Column("owner_id")]
    public Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public KickStatUser Owner { get; set; }
}