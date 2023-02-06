using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;


namespace KickStat;

public static class EnumHelper
{
    /// <summary>
    /// Получить словарь "значение поля - значение атрибута Display поля"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<EnumItem> GetEnumDisplayNames<T>()
    {
        var enumType = typeof(T);
        if (!enumType.IsEnum)
            throw new InvalidOperationException($"Type {enumType} is not enumeration");

        var enumItems = new List<EnumItem>(4);
        foreach (var val in Enum.GetValues(enumType))
        {
            FieldInfo? field = val.GetType().GetField(val.ToString()!);

            var fieldName = Enum.GetName(enumType, val);
            if (fieldName == null) continue;

            if (field == null)
            {
                enumItems.Add(new EnumItem(fieldName, val, fieldName));
                continue;
            }

            object[] attribs = field.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (attribs.Length > 0)
            {
                var displayName = ((DisplayAttribute)attribs[0]).GetName();
                enumItems.Add(new EnumItem(fieldName, val, displayName));
            }
            else
            {
                enumItems.Add(new EnumItem(fieldName, val, fieldName));
            }
        }

        return enumItems;
    }


    public static string GetDescription(Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var descriptionAttributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (descriptionAttributes == null)
            return string.Empty;

        return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : value.ToString();
    }

    public static string GetDisplayName(Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var descriptionAttributes = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

        if (descriptionAttributes == null || descriptionAttributes.Length == 0)
            return value.ToString();

        return descriptionAttributes[0].Name ?? value.ToString();
    }

    public static string ToJson(this ICollection<EnumItem> enumItems, bool propertiesInCamelCase = true)
    {
        if (propertiesInCamelCase)
            return JsonSerializer.Serialize(enumItems, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return JsonSerializer.Serialize(enumItems);
    }
}


public class EnumItem
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public object Value { get; set; }

    public EnumItem(string name, object value, string? displayName = null)
    {
        this.Name = name;
        this.Value = value;
        this.DisplayName = displayName ?? name;
    }
}