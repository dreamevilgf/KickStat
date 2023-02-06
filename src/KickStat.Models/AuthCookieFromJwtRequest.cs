using System.ComponentModel.DataAnnotations;

namespace KickStat.Models;

public class AuthCookieFromJwtRequest
{
    [Required]
    public string AccessToken { get; set; } = null!;
}