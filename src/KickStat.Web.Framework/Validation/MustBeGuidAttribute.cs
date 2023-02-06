using System.ComponentModel.DataAnnotations;

namespace KickStat.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class MustBeGuidAttribute : RegularExpressionAttribute
{
    public MustBeGuidAttribute() : base("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")
    {
    }
}