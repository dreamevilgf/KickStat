using System.Reflection;
using System.Text.Json;


namespace KickStat;

public static class JsonSerializerExt
{
    private static readonly MethodInfo _populateMethodInfo = typeof(JsonSerializerExt).GetMethod(nameof(Populate))!;

    public static void Populate<T>(T sourceObj, string jsonString) where T : class
    {
        if (sourceObj == null)
            throw new ArgumentNullException(nameof(sourceObj));

        var fromJsonObj = JsonSerializer.Deserialize<T>(jsonString);
        if (fromJsonObj == null)
            return;

        var sourceObjProperties = sourceObj.GetType().GetProperties();

        foreach (var jsonObjProperty in fromJsonObj.GetType().GetProperties())
        {
            var jsonObjPropertyValue = jsonObjProperty.GetValue(fromJsonObj);
            if (sourceObjProperties.Any(x => x.Name == jsonObjProperty.Name && jsonObjPropertyValue != null))
            {
                if (jsonObjProperty.GetType().IsClass && jsonObjProperty.PropertyType.Assembly.FullName == typeof(T).Assembly.FullName)
                {
                    MethodInfo genericPopulateMethod = _populateMethodInfo.MakeGenericMethod(jsonObjPropertyValue!.GetType());
                    var nestedObj = genericPopulateMethod.Invoke(null, new[] { jsonObjPropertyValue, JsonSerializer.Serialize(jsonObjPropertyValue) });
                    if (nestedObj != null)
                    {
                        foreach (var nestedObjProperty in nestedObj.GetType().GetProperties())
                        {
                            var nestedObjPropertyValue = nestedObjProperty.GetValue(nestedObj);
                            if (nestedObjPropertyValue != null)
                                jsonObjPropertyValue.GetType().GetProperty(nestedObjProperty.Name)?.SetValue(jsonObjProperty.GetValue(sourceObj), nestedObjPropertyValue);
                        }
                    }
                }
                else
                {
                    jsonObjProperty.SetValue(sourceObj, jsonObjPropertyValue);
                }
            }
        }
    }
}