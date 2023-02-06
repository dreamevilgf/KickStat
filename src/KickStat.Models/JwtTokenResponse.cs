namespace KickStat.Models;

public class JwtTokenResponse
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiresIn { get; set; }
}