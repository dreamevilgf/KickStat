using System.Globalization;
using System.Text.RegularExpressions;


namespace KickStat;

public static class RegexUtilities
{
    public static bool IsValidPhone(string phone)
    {
        string pureDigits = phone
            .Replace("+", "")
            .Replace(" ", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("-", "");

        return int.TryParse(pureDigits, out _);

        // try
        // {
        //     return Regex.IsMatch(phone,
        //         @"\+?\(?([0-9]{0,3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})",
        //         RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        // }
        // catch (RegexMatchTimeoutException)
        // {
        //     return false;
        // }
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}