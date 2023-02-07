using KickStat.Data;
using KickStat.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace KickStat.Services;

public class GameEventService
{
    private readonly KickStatDbContext _dbContext;

    public GameEventService(KickStatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GameEvent>> List(List<int> gameIds, List<int>? eventDetailIds = null)
    {
        if (!gameIds.Any())
            return new List<GameEvent>();

        var query = _dbContext.GameEvents.AsNoTracking().Where(x => gameIds.Contains(x.GameId));

        if (eventDetailIds.Any())
            query = query.Where(x => eventDetailIds.Contains(x.EventDetailId));

        return await query.ToListAsync();
    }
}