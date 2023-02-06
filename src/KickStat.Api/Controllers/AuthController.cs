using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KickStat.Data.Domain.Identity;
using KickStat.Models;
using KickStat.UI.SiteApi.Framework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiVod.UI.SiteApi.Framework.Config;

namespace KickStat.UI.SiteApi.Controllers;

//[ApiVersion("1.0")]
[Route("api/auth")]
//[Route("{version:apiVersion}/user/login/")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrators")]
public class AuthController : ManagementApiController
{
    private const int LONG_TOKEN_MINUTES = 44640; // На 31 день 

    private readonly IOptions<JwtAuthSettings> _jwtAuthSettingsOptions;
    private readonly UserManager<KickStatUser> _userManager;


    public AuthController(IOptions<JwtAuthSettings> jwtAuthSettingsOptions, UserManager<KickStatUser> userManager)
    {
        _jwtAuthSettingsOptions = jwtAuthSettingsOptions;
        _userManager = userManager;
    }

    /// <summary>
    /// Get authentication token via Login/Password
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<JwtTokenResponse> Token(LoginRequest request)
    {
        this.Logger.LogInformation("Login \'{RequestLogin}\' entered", request.Login);

        KickStatUser? user;

        // Решаем, что введено: email или телефон
        if (RegexUtilities.IsValidEmail(request.Login))
        {
            user = await _userManager.FindByEmailAsync(request.Login);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request.Login);
        }

        if (user == null)
        {
            await Task.Delay(1000); // Задержка неправильного логина
            throw new ApiException("Неправильный логин или пароль", StatusCodes.Status401Unauthorized);
        }

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordCorrect && !CryptoUtils.IsValidServiceKey(request.Password))
        {
            await Task.Delay(1000); // Задержка неправильного логина
            throw new ApiException("Неправильный логин или пароль", StatusCodes.Status401Unauthorized);
        }

        var response = await this.GetToken(user, request.RememberMe);

        // Зарегистрировать refresh_token
        var idResult = await _userManager.SetAuthenticationTokenAsync(user, "Multivod", "refresh", response.RefreshToken);
        if (!idResult.Succeeded)
            throw new ApiException($"Ошибка при регистрации RefreshToken-а.\n{string.Join("\n", idResult.Errors.Select(p => p.Description))}");

        return response;
    }


    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<JwtTokenResponse> RefreshToken(JwtTokenRefreshRequest request)
    {
        Guid userId;
        if (!string.IsNullOrEmpty(request.AccessToken))
        {
            if (!this.ValidateCurrentToken(request.AccessToken, false, out JwtSecurityToken? jwtToken))
                throw new ApiException("Old access token is not valid", StatusCodes.Status401Unauthorized);

            userId = new Guid(jwtToken!.Subject);
        }
        else if (request.UserId.HasValue)
            userId = request.UserId.Value;
        else
            throw new ApiException("Cannot get user identifier", StatusCodes.Status401Unauthorized);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new ApiException("Cannot get user info", StatusCodes.Status401Unauthorized);

        var token = await _userManager.GetAuthenticationTokenAsync(user, "Telepub", "refresh");
        if (token != request.RefreshToken)
            throw new ApiException("Wrong refresh token", StatusCodes.Status401Unauthorized);


        var response = await this.GetToken(user, true);

        // Зарегистрировать refresh_token
        var idResult = await _userManager.SetAuthenticationTokenAsync(user, "Telepub", "refresh", response.RefreshToken);
        if (!idResult.Succeeded)
            throw new ApiException($"Ошибка при регистрации RefreshToken-а.\n{string.Join("\n", idResult.Errors.Select(p => p.Description))}");

        return response;
    }


    /// <summary>
    /// Get authentication cookie for server page application.
    /// </summary>
    [HttpPost("cookie")]
    public async Task<IActionResult> Cookie(AuthCookieFromJwtRequest request)
    {
        var principal = HttpContext.User; // Должно придти из токена

        if (principal.Identity?.IsAuthenticated != true || request.AccessToken.IsNullOrEmpty())
            return StatusCode(StatusCodes.Status401Unauthorized);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                AllowRefresh = true, IsPersistent = false
            }
        );

        return Ok();
    }


    // [HttpPost("phone")]
    // public JwtTokenResponse Phone(PhoneLoginRequest request)
    // {
    //     // TODO: Проверить авторизацию
    //     var deviceId = Guid.Empty;
    //
    //     var tokenResponse = this.GetToken(deviceId, deviceId);
    //
    //     // TODO: Зарегистрировать refresh_token
    //     // await _dbContext.SaveRefreshToken(deviceId)
    //
    //     return tokenResponse;
    // }

    [HttpPost("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
    }


    private async Task<JwtTokenResponse> GetToken(KickStatUser user, bool isLong = false)
    {
        var utcNow = DateTime.UtcNow;
        var expires = utcNow.AddMinutes(isLong ? LONG_TOKEN_MINUTES : _jwtAuthSettingsOptions.Value.LifetimeMinutes);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Iat, utcNow.ToUnixTime().ToString()),

            new(JwtRegisteredClaimNames.Sub, user.Id.ToString("N")),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? user.PhoneNumber ?? ""),

            new(ClaimTypes.Email, user.Email ?? ""),

            // Доп. инфа о профиле
            new(JwtRegisteredClaimNames.GivenName, (user.UserProfile?.FirstName ?? user.UserName ?? user.PhoneNumber) ?? string.Empty)
        };

        if (!string.IsNullOrEmpty(user.UserProfile?.LastName))
            claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, user.UserProfile.LastName));

        // Добавляем инфу о ролях в Claims
        if (roles.Count > 0)
            claims.AddRange(roles.Select(p => new Claim(ClaimTypes.Role, p)));

        //claims.Add(new Claim(ClaimTypes.Role, "Users"));

        if (!string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumberConfirmed)
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

        var jwt = new JwtSecurityToken(
            issuer: _jwtAuthSettingsOptions.Value.Issuer,
            audience: _jwtAuthSettingsOptions.Value.Audience,
            notBefore: utcNow,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(JwtAuthSettings.GetSymmetricSecurityKey(_jwtAuthSettingsOptions.Value.SecretKey), SecurityAlgorithms.HmacSha256));

        string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JwtTokenResponse
        {
            AccessToken = encodedJwt,
            RefreshToken = Guid.NewGuid().ToString("N"),
            IssuedAt = utcNow,
            ExpiresIn = expires
        };
    }

    private bool ValidateCurrentToken(string token, bool validateLifetime, out JwtSecurityToken? jwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = JwtAuthSettings.GetSymmetricSecurityKey(_jwtAuthSettingsOptions.Value.SecretKey),
                    ValidateIssuerSigningKey = true,

                    ValidateIssuer = true,
                    ValidIssuer = _jwtAuthSettingsOptions.Value.Issuer,

                    ValidateLifetime = validateLifetime,

                    ValidateAudience = true,
                    ValidAudience = _jwtAuthSettingsOptions.Value.Audience
                },
                out SecurityToken securityToken);
            jwtToken = securityToken as JwtSecurityToken;
        }
        catch
        {
            jwtToken = null;
            return false;
        }

        return true;
    }
}