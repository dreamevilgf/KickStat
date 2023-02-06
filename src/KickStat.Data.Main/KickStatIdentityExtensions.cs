
using KickStat.Data.Domain.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KickStat.Data;

public static class KickStatIdentityExtensions
{

    public static IServiceCollection AddKickStatIdentity(this IServiceCollection services, string loginUrl = "/account/login", string logoutUrl = "/account/logout",
        string accesDeniedPath = "/admin/account/accessDenied")
    {
        AddMultiVodIdentity(services, options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.MaxFailedAccessAttempts = 30;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        });


        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            options.Cookie.HttpOnly = false;
            options.Cookie.Name = ".authTicket";
            options.Cookie.Path = "/";
            options.LoginPath = loginUrl;
            options.LogoutPath = logoutUrl;
            options.AccessDeniedPath = accesDeniedPath;

            // Для JSON-запросов отдаем 401 код вместо
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = ctx =>
                {
                    string accept = ctx.Request.Headers["Accept"]!;
                    bool isJsonRequest = accept?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true;

                    if (isJsonRequest && ctx.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        ctx.Response.Headers["Content-Type"] = "application/json; charset=utf-8";
                        ctx.Response.Headers["Cache-Control"] = "no-cache, no-store";

                        ctx.Response.WriteAsync($"{{\"error\": \"Доступ закрыт.\", \"additionalData\": \"{ctx.RedirectUri}\"}}");
                    }
                    else if (!isJsonRequest)
                        ctx.Response.Redirect(ctx.RedirectUri);

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddMultiVodIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction)
    {
        services
            .AddIdentity<KickStatUser, KickStatRole>(setupAction)
            .AddEntityFrameworkStores<KickStatDbContext>()
            .AddDefaultTokenProviders();

        services.RemoveAll<IPasswordHasher<KickStatUser>>();
        services.TryAddScoped<IPasswordHasher<KickStatUser>, PasswordHasherMembershipCompatible>();
        return services;
    }


    public static IApplicationBuilder UseKickStatIdentity(this IApplicationBuilder app)
    {
        return app.UseAuthentication().UseAuthorization();
    }
}