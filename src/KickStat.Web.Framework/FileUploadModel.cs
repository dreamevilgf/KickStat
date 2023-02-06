namespace KickStat;

public class FileUploadModel
{
    public string? Name { get; set; }

    public int Size { get; set; }

    public string? Type { get; set; }

    /// <summary>
    /// Сериализовано как Base64-строка
    /// </summary>
    public string? Data { get; set; }


    public byte[] GetDataBytes()
    {
        // Данные приходят в формате base64,type:text/plain,234234HEFA567966796.....
        // берем только данные

        if (string.IsNullOrEmpty(this.Data))
            return Array.Empty<byte>();

        int lastIndexOfComma = this.Data.LastIndexOf(",", StringComparison.OrdinalIgnoreCase);
        if (lastIndexOfComma == -1)
            return Convert.FromBase64String(this.Data);

        string dataCut = this.Data.Substring(lastIndexOfComma + 1);
        return Convert.FromBase64String(dataCut);
    }
}