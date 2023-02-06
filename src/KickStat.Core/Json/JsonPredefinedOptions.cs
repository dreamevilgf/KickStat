using System.Text.Json;
using KickStat.Json;


namespace FarPlan.Json;

public static class JsonPredefinedOptions
{
    public static readonly JsonSerializerOptions AppGeneral = new(JsonSerializerDefaults.Web);

    public static readonly JsonSerializerOptions ThirdParty = new(JsonSerializerDefaults.Web);
        
        
    static JsonPredefinedOptions()
    {
        var versionConverter = new VersionConverter();
            
        AppGeneral.Converters.Add(versionConverter);
        ThirdParty.Converters.Add(versionConverter);
    }

}