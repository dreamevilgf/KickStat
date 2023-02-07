using KickStat.Data;
using KickStat.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KickStat.Services;

public class EventDetailService
{
    private readonly IMemoryCache _memoryCache;
    private readonly KickStatDbContext _dbContext;

    private const string ALL_EVENT_DETAILS_KEY = "KickStat.AllEventDetails";

    public EventDetailService(IMemoryCache memoryCache, KickStatDbContext dbContext)
    {
        _memoryCache = memoryCache;
        _dbContext = dbContext;
    }

    public async ValueTask<List<EventDetail>> All()
    {
        var result = _memoryCache.Get<List<EventDetail>>(ALL_EVENT_DETAILS_KEY);

        if (result != null)
            return result;

        result = await _dbContext.EventDetails.AsNoTracking().ToListAsync();

        _memoryCache.Set(result, TimeSpan.FromHours(1));

        return result;
    }
}