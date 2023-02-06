using System.ComponentModel.DataAnnotations;

namespace KickStat.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Необходимо указать email")]
    [EmailAddress(ErrorMessage = "Необходимо указать email")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Необходимо указать пароль")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}