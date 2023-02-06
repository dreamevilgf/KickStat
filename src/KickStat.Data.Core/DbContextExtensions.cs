using Microsoft.EntityFrameworkCore;

namespace KickStat.Data;

public static class DbContextExtensions
{
    public static EfRepository<T> Repository<T>(this DbContext dbContext) where T : class => new EfRepository<T>(dbContext);


    public static void Detach(this DbContext dbContext, object entity)
    {
        var changed = dbContext.ChangeTracker.Entries().FirstOrDefault(p => p.Entity == entity);
        if (changed != null)
            changed.State = EntityState.Detached;
    }


    public static T AsReadOnly<T>(this T dbContext) where T : DbContext
    {
        dbContext.Detach(1);

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


        return dbContext;
    }

    public static T AsReadWrite<T>(this T dbContext) where T : DbContext
    {
        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        return dbContext;
    }


    /// <summary>
    /// Попытаться сохранить изменения. Исключение не выбрасывается при неудачной попытке.
    /// </summary>
    /// <param name="dbContext">DbContext to extend</param>
    /// <param name="countTries">Количество попыток</param>
    /// <param name="intervalBetweenTriesMs">Временной интервал между попытками</param>
    /// <returns>null, либо последнее выброшенное исключение</returns>
    public static Exception? TrySaveChanges(this DbContext dbContext, int countTries = 1, int intervalBetweenTriesMs = 500)
    {
        if (countTries <= 0 || countTries >= 100)
            throw new ArgumentException("countTries должен быть от 1 до 100");

        Exception? lastException = null;

        for (int i = 0; i < countTries; i++)
        {
            try
            {
                dbContext.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (intervalBetweenTriesMs > 0)
                    Task.Delay(intervalBetweenTriesMs).Wait();
            }
        }

        return lastException;
    }

    public static async Task<Exception?> TrySaveChangesAsync(this DbContext dbContext, int countTries = 1, int intervalBetweenTriesMs = 500,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (countTries <= 0 || countTries >= 100)
            throw new ArgumentException("countTries должен быть от 1 до 100");

        Exception? lastException = null;

        for (int i = 0; i < countTries; i++)
        {
            bool needWait = false;
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return null;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (intervalBetweenTriesMs > 0)
                    needWait = true;
            }

            if (needWait)
                await Task.Delay(intervalBetweenTriesMs, cancellationToken).ConfigureAwait(false);
        }

        return lastException;
    }
}