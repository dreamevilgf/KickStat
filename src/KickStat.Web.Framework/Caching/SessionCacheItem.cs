namespace KickStat.Caching;

public class SessionCacheItem
{
    public SessionCacheItem(object data, DateTime expireAtUtc)
    {
        ExpireAt = expireAtUtc;
        Data = data;
    }

    public DateTime ExpireAt;

    public object Data;
}