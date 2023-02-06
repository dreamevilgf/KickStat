using KickStat.Data.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KickStat.Data.Domain.Identity;

[Table("Users_Profile_Settings")]
public class UserProfileSetting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Required]
    [Column("value")]
    public string Value { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public KickStatUser? User { get; set; }
}