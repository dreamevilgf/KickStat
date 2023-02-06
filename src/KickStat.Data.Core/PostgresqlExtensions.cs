using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace KickStat.Data;

public static class PostgresqlExtensions
{
    /// <summary>
    /// Sets autoincrement sequence to max value of table. May be used after non empty autoincrement field insert.
    /// </summary>
    public static Task<int> SetSequenceToMaxValueAsync<TEntity, TProperty>(this DbSet<TEntity> dbSet, DbContext dbContext, Expression<Func<TEntity, TProperty>> memberExpression)
        where TEntity : class
    {
        string? columnName = dbSet.GetColumnName(dbContext, memberExpression);
        string? tableName = dbSet.GetTableName(dbContext);

        if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(tableName))
            return Task.FromResult(1);
            
        string
            sql = $"select setval(pg_get_serial_sequence('{tableName}', '{columnName}'), (select max({columnName}) from {tableName}));"; // если запердолить строку внутрь метода, работать не будет

#pragma warning disable EF1000
        return dbContext.Database.ExecuteSqlRawAsync(sql);
#pragma warning restore EF1000
    }
}