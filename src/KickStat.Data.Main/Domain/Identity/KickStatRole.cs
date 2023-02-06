using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KickStat.Data.Domain.Identity;

public class KickStatRole : IdentityRole<Guid> //, ManagerUserInRole, ManagerRoleClaim>
{
    [MaxLength(256)]
    [Column("display_name")]
    public string? DisplayName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    public virtual ICollection<KickStatUser> Users { get; set; } = new HashSet<KickStatUser>();
}