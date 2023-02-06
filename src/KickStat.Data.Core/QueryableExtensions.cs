using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace KickStat.Data;

public static class QueryableExtensions
{
    public static T? FirstOrDefaultReadUncommitted<T>(this IQueryable<T> query) => AsReadUncommitted(query.FirstOrDefault);

    public static T[] ToArrayReadUncommitted<T>(this IQueryable<T> query) => AsReadUncommitted(query.ToArray);

    public static List<T> ToListReadUncommitted<T>(this IQueryable<T> query) => AsReadUncommitted(query.ToList);

    public static int CountReadUncommitted<T>(this IQueryable<T> query) => AsReadUncommitted(query.Count);


    public static async Task<T?> FirstOrDefaultReadUncommittedAsync<T>(this IQueryable<T> query) => await AsReadUncommittedAsync(() => query.FirstOrDefaultAsync());

    public static async Task<T[]> ToArrayReadUncommittedAsync<T>(this IQueryable<T> query) => await AsReadUncommittedAsync(() => query.ToArrayAsync());

    public static async Task<List<T>> ToListReadUncommittedAsync<T>(this IQueryable<T> query) => await AsReadUncommittedAsync(() => query.ToListAsync());

    public static async Task<int> CountReadUncommittedAsync<T>(this IQueryable<T> query) => await AsReadUncommittedAsync(() => query.CountAsync());


    public static async Task<TResult> AsReadUncommittedAsync<TResult>(Func<Task<TResult>> func)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        TResult toReturn = await func();
        scope.Complete();
        return toReturn;
    }

    public static TResult AsReadUncommitted<TResult>(Func<TResult> func)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        TResult toReturn = func();
        scope.Complete();
        return toReturn;
    }
}