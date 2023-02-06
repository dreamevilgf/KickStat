using Microsoft.EntityFrameworkCore;

namespace KickStat.Data;

public interface IRepository<T> where T : class
{
    DbContext DbContext { get; }
    DbSet<T> DbSet { get; }
    IQueryable<T> Query { get; }

    T MarkToAdd(T entity);
    ICollection<T> MarkToAdd(ICollection<T> entities);

    T MarkToUpdate(T entity);

    void MarkToDelete(T entity);
    void MarkToDelete(ICollection<T> entities);

    /// <summary>
    ///     Finds an entity with the given primary key values. If an entity with the given primary key values
    ///     is being tracked by the context, then it is returned immediately without making a request to the
    ///     database. Otherwise, a query is made to the database for an entity with the given primary key values
    ///     and this entity, if found, is attached to the context and returned. If no entity is found, then
    ///     null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>The entity found, or null.</returns>
    T? Find(params object[] keyValues);

    /// <summary>
    ///     Finds an entity with the given primary key values. If an entity with the given primary key values
    ///     is being tracked by the context, then it is returned immediately without making a request to the
    ///     database. Otherwise, a query is made to the database for an entity with the given primary key values
    ///     and this entity, if found, is attached to the context and returned. If no entity is found, then
    ///     null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>The entity found, or null.</returns>
    ValueTask<T?> FindAsync(params object[] keyValues);

    T Add(T entity);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    T Update(T entity);

    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    void Delete(T entity);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    int SaveMarkedChanges();

    Task<int> SaveMarkedChangesAsync(CancellationToken cancellationToken = default);

    Exception? TrySaveMarkedChanges(int countTries = 1, int intervalBetweenTriesMs = 500);

    Task<Exception?> TrySaveMarkedChangesAsync(int countTries = 1, int intervalBetweenTriesMs = 500, CancellationToken cancellationToken = default);
}