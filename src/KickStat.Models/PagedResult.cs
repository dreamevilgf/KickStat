using System.Text.Json.Serialization;

namespace KickStat.Models;

public class PagedResult<T>
{
    [JsonPropertyName("result")]
    public IList<T>? Result { get; set; }

    [JsonPropertyName("totalCount")]
    public int? TotalCount { get; set; }

    [JsonPropertyName("skip")]
    public int? Skip { get; set; }

    [JsonPropertyName("take")]
    public int? Take { get; set; }

    public PagedResult() { }

    public PagedResult(IList<T>? result, int? totalCount = null, int? skip = null, int? take = null)
    {
        this.Result = result;

        if (result != null && totalCount == null)
            this.TotalCount = result.Count;
        else if (totalCount.HasValue)
            this.TotalCount = totalCount.Value;

        this.Skip = skip;
        this.Take = take;
    }
}