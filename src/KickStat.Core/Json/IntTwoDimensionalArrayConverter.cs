using System.Text.Json;
using System.Text.Json.Serialization;


namespace FarPlan.Json;

public class IntTwoDimensionalArrayConverter : JsonConverter<int[,]?>
{
    public override int[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.StartArray)
            reader.Read();

        if (reader.TokenType == JsonTokenType.EndArray)
            return new int[0, 0];

        var resultList = new List<List<int>>();
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            var dimensionResult = new List<int>();
            if (reader.TokenType == JsonTokenType.StartArray)
                reader.Read();

            while (reader.TokenType == JsonTokenType.Number)
            {
                dimensionResult.Add(reader.GetInt32());
                reader.Read();
            }

            if (reader.TokenType == JsonTokenType.EndArray)
                reader.Read();

            resultList.Add(dimensionResult);
        }

        int[,] resultArr = new int[resultList.Count, resultList.FirstOrDefault()?.Count ?? 0];
        for (int i = 0; i < resultList.Count; i++)
        for (int j = 0; j < resultList[i].Count; j++)
            resultArr[i, j] = resultList[i][j];

        return resultArr;
    }

    public override void Write(Utf8JsonWriter writer, int[,]? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        for (int i = 0; i < value.GetLength(0); i++)
        {
            writer.WriteStartArray();

            for (int j = 0; j < value.GetLength(1); j++)
                writer.WriteNumberValue(value[i, j]);

            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }
}