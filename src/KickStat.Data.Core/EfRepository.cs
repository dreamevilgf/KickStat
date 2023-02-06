using Microsoft.EntityFrameworkCore;

namespace KickStat.Data;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public EfRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public DbContext DbContext => _dbContext;

    public DbSet<T> DbSet => _dbSet;

    public IQueryable<T> All => _dbSet;

    public IQueryable<T> Query => _dbSet;

    public T MarkToAdd(T entity)
    {
        _dbSet.Add(entity);
        return entity;
    }

    public ICollection<T> MarkToAdd(ICollection<T> entities)
    {
        bool isAutoDetectWasEnabled = _dbContext.ChangeTracker.AutoDetectChangesEnabled;
        if (isAutoDetectWasEnabled)
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        foreach (var entity in entities)
            _dbSet.Add(entity);

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = isAutoDetectWasEnabled;
        if (isAutoDetectWasEnabled)
            _dbContext.ChangeTracker.DetectChanges();

        return entities;
    }

    public ICollection<T> MarkToUpdate(ICollection<T> entities)
    {
        bool isAutoDetectWasEnabled = _dbContext.ChangeTracker.AutoDetectChangesEnabled;
        if (isAutoDetectWasEnabled)
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

        foreach (var entity in entities)
        {
            var entityEntry = _dbSet.Attach(entity);
            entityEntry.State = EntityState.Modified;
        }

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = isAutoDetectWasEnabled;

        if (isAutoDetectWasEnabled)
            _dbContext.ChangeTracker.DetectChanges();

        return entities;
    }

    public T MarkToUpdate(T entity)
    {
        if (_dbContext.ChangeTracker.Entries<T>().All(p => p.Entity != entity))
        {
            var entityEntry = _dbSet.Attach(entity);
            entityEntry.State = EntityState.Modified;
        }

        return entity;
    }

    public void MarkToDelete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void MarkToDelete(ICollection<T> entities)
    {
        bool isAutoDetectWasEnabled = _dbContext.ChangeTracker.AutoDetectChangesEnabled;
        if (isAutoDetectWasEnabled)
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        foreach (var entity in entities)
            MarkToDelete(entity);

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = isAutoDetectWasEnabled;
        if (isAutoDetectWasEnabled)
            _dbContext.ChangeTracker.DetectChanges();
    }

    /// <inheritdoc />
    public T? Find(params object[] keyValues) => _dbSet.Find(keyValues);

    /// <inheritdoc />
    public ValueTask<T?> FindAsync(params object[] keyValues) => _dbSet.FindAsync(keyValues); // По документации, может возвращаться null, возможно, косяк Ef Core.

    public T Add(T entity)
    {
        MarkToAdd(entity);
        _dbContext.SaveChanges();

        return entity;
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        MarkToAdd(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return entity;
    }

    public T Update(T entity)
    {
        MarkToUpdate(entity);

        _dbContext.SaveChanges();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        MarkToUpdate(entity);

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity;
    }

    public void Delete(T entity)
    {
        MarkToDelete(entity);
        _dbContext.SaveChanges();
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        MarkToDelete(entity);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public int SaveMarkedChanges() => _dbContext.SaveChanges();

    public async Task<int> SaveMarkedChangesAsync(CancellationToken cancellationToken = default) => await _dbContext.SaveChangesAsync(cancellationToken);

    public Exception? TrySaveMarkedChanges(int countTries = 1, int intervalBetweenTriesMs = 500)
    {
        if (countTries <= 0 || countTries >= 100)
            throw new ArgumentException("countTries должен быть от 1 до 100");

        Exception? lastException = null;

        for (int i = 0; i < countTries; i++)
        {
            try
            {
                _dbContext.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                lastException = ex;
                if (intervalBetweenTriesMs > 0)
                {
                }

                Task.Delay(intervalBetweenTriesMs).Wait();
            }
        }

        return lastException;
    }


    public async Task<Exception?> TrySaveMarkedChangesAsync(int countTries = 1, int intervalBetweenTriesMs = 500, CancellationToken cancellationToken = default)
    {
        if (countTries <= 0 || countTries >= 100)
            throw new ArgumentException("countTries должен быть от 1 до 100");

        Exception? lastException = null;

        for (int i = 0; i < countTries; i++)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return null;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            // Если мы здесь, значит ошибка была
            if (intervalBetweenTriesMs > 0)
                await Task.Delay(intervalBetweenTriesMs, cancellationToken);
        }

        return lastException;
    }
}