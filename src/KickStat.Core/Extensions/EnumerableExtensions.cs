using System.Collections.Concurrent;


namespace KickStat;

public static class EnumerableExtensions
{
    /// <summary>
    /// Получить первый индекс элемента в последовательности по некоторому условию. Если не найдено, возврат -1
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sequence">последовательность</param>
    /// <param name="predicate">условие выбора</param>
    /// <returns></returns>
    public static int FindFirstIndex<TSource>(this IEnumerable<TSource> sequence, Func<TSource, bool> predicate)
    {
        int i = 0;
        foreach (var item in sequence)
        {
            if (predicate(item))
                return i;

            i++;
        }

        return -1;
    }

    /// <summary>
    /// Получить первый индекс элемента в последовательности по некоторому условию. Если не найдено, возврат -1
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sequence">последовательность</param>
    /// <param name="item">Элемент, для которого нужно провести сравнение</param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FindFirstIndex<TSource>(this IEnumerable<TSource> sequence, TSource item, IEqualityComparer<TSource>? comparer = null)
    {
        if (item == null)
            return -1;

        if (comparer == null)
            comparer = EqualityComparer<TSource>.Default;

        int i = 0;
        foreach (var sequenceItem in sequence)
        {
            if (comparer.Equals(sequenceItem, item))
                return i;

            i++;
        }

        return -1;
    }


    /// <summary>
    /// Получить последний индекс элемента в последовательности по некоторому условию. Если не найдено, возврат -1
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sequence">последовательность</param>
    /// <param name="predicate">условие выбора</param>
    /// <returns></returns>
    public static int FindLastIndex<TSource>(this IEnumerable<TSource> sequence, Func<TSource, bool> predicate)
    {
        var array = sequence.ToArray();

        int i;
        for (i = array.Length - 1; i >= 0; i--)
        {
            if (predicate(array[i]))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Получить последний индекс элемента в последовательности по некоторому условию. Если не найдено, возврат -1
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sequence">Последовательность</param>
    /// <param name="item">Элемент, для которого нужно провести сравнение</param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int FindLastIndex<TSource>(this IEnumerable<TSource> sequence, TSource item, IEqualityComparer<TSource>? comparer = null)
    {
        if (item == null)
            return -1;

        if (comparer == null)
            comparer = EqualityComparer<TSource>.Default;

        var array = sequence.ToArray();

        int i;
        for (i = array.Length - 1; i >= 0; i--)
        {
            if (comparer.Equals(array[i], item))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Execute asynchronously code on every element of collection
    /// </summary>
    /// <param name="source"></param>
    /// <param name="degreeOfParallelism">Maximum number of concurrent threads</param>
    /// <param name="body">Body of something to execute</param>
    /// <param name="exHandler">Exception handler</param>
    public static Task ForEachAsync<T>(this IEnumerable<T> source, int degreeOfParallelism, Func<T, Task> body, Action<AggregateException, T>? exHandler = null)
    {
        return Task.WhenAll(
            from partition in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
            select Task.Run(async delegate
            {
                using (partition)
                    while (partition.MoveNext())
                        await body(partition.Current).ContinueWith(t =>
                        {
                            if (t.Exception != null && exHandler != null)
                                exHandler(t.Exception, partition.Current);
                        });
            }));
    }
}