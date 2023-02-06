using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace KickStat.Routing;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public const string SLUGIFY = "slugify";

    public string? TransformOutbound(object? value) =>
        value == null
            ? null
            : Regex.Replace(value.ToString() ?? "", "([a-z])([A-Z])", "$1-$2", RegexOptions.Compiled).ToLower();
}