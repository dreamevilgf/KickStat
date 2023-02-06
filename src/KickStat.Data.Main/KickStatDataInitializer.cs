using KickStat.Data.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace KickStat.Data;

public partial class KickStatDbContext
{
    /// <summary>
    /// Создать админского пользователя и роль. Пароль по умолчанию - Admin01!
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public async Task EnsureAdminCreated(UserManager<KickStatUser> userManageService, RoleManager<KickStatRole> roleManageService)
    {
        var adminRole = await roleManageService.FindByNameAsync("Administrator");
        if (adminRole == null)
        {
            adminRole = new KickStatRole { Name = "Administrator", Description = "Administrators can manage users and roles" };
            var result = await roleManageService.CreateAsync(adminRole);
            if (result != IdentityResult.Success)
                throw new Exception($"Ошибка. Роль Administrators не создана.\n{string.Join("\n", result.Errors.Select(p => p.Description))}");
        }


        var adminUser = await userManageService.FindByNameAsync("Admin");
        if (adminUser == null)
        {
            adminUser = new KickStatUser
            {
                UserName = "Admin",
                Email = "admin@kickstat.ru",
                LockoutEnabled = false,
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow
            };
            var result = await userManageService.CreateAsync(adminUser, "Admin01!");
            if (result != IdentityResult.Success)
                throw new Exception($"Ошибка. Пользователь Admin не создан.\n{string.Join("\n", result.Errors.Select(p => p.Description))}");
        }

        // Проверяем, что он в роли администраторов
        bool isInAdminRole = await userManageService.IsInRoleAsync(adminUser, adminRole.Name!);
        if (!isInAdminRole)
        {
            var result = await userManageService.AddToRoleAsync(adminUser, adminRole.Name!);
            if (result != IdentityResult.Success)
                throw new ApplicationException(
                    $"Ошибка. Пользователь '{adminUser.UserName}' не добавлен к роли '{adminRole.Name}'.\n{string.Join("\n", result.Errors.Select(p => p.Description))}");
        }
    }
}