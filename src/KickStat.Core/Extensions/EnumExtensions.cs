using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KickStat;

public static class EnumExtensions
{
    public static Dictionary<string, int> GetNameValueDictionary(this Enum enumeration, Type enumType)
    {
        var allValues = Enum.GetValues(enumType);

        var result = new Dictionary<string, int>(allValues.Length);
        foreach (var value in allValues)
        {
            string? name = Enum.GetName(enumType, value);
            if (name != null)
                result.Add(name, (int)value);
        }

        return result;
    }
    
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()?
            .GetMember(enumValue.ToString())?[0]?
            .GetCustomAttribute<DisplayAttribute>()?
            .Name ?? "Нет значения";
    }
    
}