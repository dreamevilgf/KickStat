using System.Text.Json;
using System.Text.Json.Serialization;

namespace KickStat.Json;

public class VersionConverter : JsonConverter<Version?>
{
    public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? versionString = reader.GetString();
        if (Version.TryParse(versionString, out Version? result))
        {
            return result;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, Version? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString());
    }
}