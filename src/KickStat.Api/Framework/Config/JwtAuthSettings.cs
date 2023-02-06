using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MultiVod.UI.SiteApi.Framework.Config;

public class JwtAuthSettings
{
    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public int LifetimeMinutes { get; set; }

    public string SecretKey { get; set; } = null!;


    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key) => new(Encoding.ASCII.GetBytes(key));
}