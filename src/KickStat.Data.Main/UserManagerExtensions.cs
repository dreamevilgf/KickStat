using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;

namespace MultiVod.Data;

public static class UserManagerExtensions
{
    private static readonly Random _random = new((int)DateTime.Now.Ticks);
    private static readonly PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();


    public static string NormalizePhone<TUser>(this UserManager<TUser> userManager, string phone, string defaultRegion = "ru") where TUser : IdentityUser
    {
        PhoneNumber number = _phoneNumberUtil.Parse(phone, defaultRegion);
        return _phoneNumberUtil.Format(number, PhoneNumberFormat.E164);
    }

    public static async Task<TUser?> FindByPhoneAsync<TUser>(this UserManager<TUser> userManager, string phone, string defaultRegion = "ru") where TUser : IdentityUser<Guid>
    {
        PhoneNumber number;
        try
        {
            number = _phoneNumberUtil.Parse(phone, defaultRegion);
        }
        catch (Exception)
        {
            return null;
        }

        string normalizedPhone = _phoneNumberUtil.Format(number, PhoneNumberFormat.E164);
        return await userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == normalizedPhone);
    }

    public static string GeneratePassword<TUser>(this UserManager<TUser> userManager) where TUser : class =>
        GeneratePassword(userManager, userManager.Options.Password.RequiredLength);

    public static string GeneratePassword<TUser>(this UserManager<TUser> userManager, int size) where TUser : class
    {
        var sBuilder = new StringBuilder();
        lock (_random)
        {
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
                sBuilder.Append(ch);
            }

            if (userManager.Options.Password.RequireDigit)
            {
                sBuilder.Append(_random.Next(1, 100));
            }
        }

        return sBuilder.ToString();
    }
}