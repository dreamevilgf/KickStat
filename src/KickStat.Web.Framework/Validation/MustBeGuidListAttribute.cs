using System.ComponentModel.DataAnnotations;

namespace KickStat.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class MustBeGuidListAttribute : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return $"Поле ${name} должно быть списком GUID-ов";
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;

        if (value is not string)
            return false;

        string strValue = (string)value;
        if (string.IsNullOrWhiteSpace(strValue))
            return true;

        var parsed = strValue.Split(new[] { ",", ";", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        return parsed.All(strGuid => Guid.TryParse(strGuid, out _));
    }
}