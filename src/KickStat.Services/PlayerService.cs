using KickStat.Data;
using KickStat.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KickStat.Services;

public class PlayerService
{
    private readonly KickStatDbContext _dbContext;
    

    public PlayerService(KickStatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<List<Player>> List(Guid ownerId)
    {
        return await _dbContext.Players.AsNoTracking().Where(x => x.OwnerId == ownerId && !x.IsDeleted).ToListAsync();
    }
}