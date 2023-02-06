using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KickStat.Data.Domain.Identity;

public class KickStatUserToken : IdentityUserToken<Guid>
{
    /// <summary>
    /// Token expiration in UTC
    /// </summary>
    [Column("expire_at")]
    public DateTimeOffset? ExpireAt { get; set; }
}