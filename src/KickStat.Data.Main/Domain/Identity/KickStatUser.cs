using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KickStat.Data.Domain.Identity;

public class KickStatUser : IdentityUser<Guid> //, ManagerUserClaim, ManagerUserInRole, ManagerUserLogin>
{
    // /// <summary>
    // /// Navigation property for the roles this user belongs to.
    // /// </summary>
    public virtual ICollection<KickStatRole> Roles { get; } = new HashSet<KickStatRole>();

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    public UserProfile? UserProfile { get; set; }

    public IList<UserProfileSetting> UserProfileSettings { get; } = new List<UserProfileSetting>();
}