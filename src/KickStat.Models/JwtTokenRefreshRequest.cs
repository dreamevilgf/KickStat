using System.ComponentModel.DataAnnotations;

namespace KickStat.Models;

public class JwtTokenRefreshRequest : IValidatableObject
{
    [Required]
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Старый токен, из которого можно вынуть информацию
    /// </summary>
    public string? AccessToken { get; set; }

    public Guid? UserId { get; set; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(AccessToken) && UserId == null)
            yield return new ValidationResult($"{nameof(AccessToken)} or {nameof(UserId)} must be set", new[] { nameof(AccessToken), nameof(UserId) });
    }
}