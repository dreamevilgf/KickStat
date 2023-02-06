using KickStat.Data.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KickStat.Data.Domain.Identity;

[Table("user_profiles")]
public class UserProfile
{
    /// <summary>
    /// Идентификатор профиля пользователя. Совпадает с идентификатором пользователя.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public Guid Id { get; set; }

    [MaxLength(64)]
    [Column("first_name")]
    public string? FirstName { get; set; }

    [MaxLength(64)]
    [Column("last_name")]
    public string? LastName { get; set; }

    [MaxLength(64)]
    [Column("middle_name")]
    public string? MiddleName { get; set; }

    [MaxLength(32)]
    [Column("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Timezone offset in hours
    /// </summary>
    [Column("time_zone_utc_offset")]
    public double? TimeZoneUtcOffset { get; set; }

    [ForeignKey(nameof(Id))]
    public KickStatUser? User { get; set; }


    public TimeZoneInfo GetTimeZone() =>
        TimeZoneUtcOffset != null
            ? TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(p => Math.Abs(p.BaseUtcOffset.TotalHours - TimeZoneUtcOffset.Value) < 0.1) ?? TimeZoneInfo.Local
            : TimeZoneInfo.Local;
}