using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KickStat.Data.Domain;

[Table("app_settings")]
public class AppSetting
{
    [Key]
    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("value")]
    public string? Value { get; set; }
}