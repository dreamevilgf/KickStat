using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;


namespace KickStat.Data;

public static class DbSetExtensions
{
    public static EfRepository<T> AsRepository<T>(this DbSet<T> set) where T : class => new EfRepository<T>(set.GetDbContext());


    public static DbContext GetDbContext<TEntity>(this DbSet<TEntity> dbSet) where TEntity : class
    {
        var infrastructure = dbSet as IInfrastructure<IServiceProvider>;
        var serviceProvider = infrastructure.Instance;
        var currentDbContext = serviceProvider.GetRequiredService<ICurrentDbContext>();
        return currentDbContext.Context;
    }


    public static string? GetTableName<TEntity>(this DbSet<TEntity> dbSet, DbContext dbContext) where TEntity : class =>
        GetTableName((IQueryable<TEntity>)dbSet, dbContext);


    public static string? GetTableName<TEntity>(this IQueryable<TEntity> dbSet, DbContext dbContext) where TEntity : class
    {
        var entityType = dbContext.Model.FindEntityType(typeof(TEntity).FullName!);
        if (entityType == null)
            throw new NullReferenceException($"No entity type found for {typeof(TEntity).FullName}");

        return entityType.GetTableName();
    }

    public static string? GetColumnName<TEntity, TProperty>(this DbSet<TEntity> dbSet, DbContext dbContext, Expression<Func<TEntity, TProperty>> propertyExpression) where TEntity : class =>
        GetColumnName((IQueryable<TEntity>)dbSet, dbContext, propertyExpression);


    public static string? GetColumnName<TEntity, TProperty>(this IQueryable<TEntity> dbSet, DbContext dbContext, Expression<Func<TEntity, TProperty>> memberExpression) where TEntity : class
    {
        var propExpr = memberExpression.Body as MemberExpression;
        if (propExpr == null)
            throw new ArgumentException("Expression must be a member expression", nameof(memberExpression));

        string propName = propExpr.Member.Name;

        var entityType = dbContext.Model.FindEntityType(typeof(TEntity).FullName!);
        if (entityType == null)
            throw new NullReferenceException($"No entity type found for {typeof(TEntity).FullName}");

        var storeObjectId = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table)
                            ?? throw new ArgumentException($"StoreObjectIdentifier.Create for {entityType.Name} returns null");

        return entityType.FindProperty(propName)?.GetColumnName(storeObjectId);
    }
}