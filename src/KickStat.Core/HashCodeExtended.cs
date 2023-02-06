namespace KickStat;

public static class HashCodeExtended
{
    public static int GetHashCode<T>(T[] list)
    {
        unchecked
        {
            int hash = 0;
            foreach (var item in list)
            {
                if (item == null) continue;
                hash = 31 * hash + item.GetHashCode();
            }

            return hash;
        }
    }

    public static int GetHashCode<T>(IEnumerable<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        unchecked
        {
            int hash = 0;
            foreach (var item in list)
            {
                if (item == null) continue;
                hash = 31 * hash + item.GetHashCode();
            }

            return hash;
        }
    }

    /// <summary>
    /// Gets a hashcode for a collection for that the order of items 
    /// does not matter.
    /// So {1, 2, 3} and {3, 2, 1} will get same hash code.
    /// </summary>
    public static int GetHashCodeForOrderNoMatterCollection<T>(IEnumerable<T> list)
    {
        unchecked
        {
            int hash = 0;
            int count = 0;
            foreach (var item in list)
            {
                if (item == null) continue;
                hash += item.GetHashCode();
                count++;
            }

            return 31 * hash + count.GetHashCode();
        }
    }

    /// <summary>
    /// Alternative way to get a hashcode is to use a fluent 
    /// interface like this:<br />
    /// return 0.CombineHashCode(field1).CombineHashCode(field2).
    ///     CombineHashCode(field3);
    /// </summary>
    public static int CombineHashCode<T>(this int hashCode, T arg)
    {
        if (arg == null)
            return hashCode;

        unchecked
        {
            return 31 * hashCode + arg.GetHashCode();
        }
    }
}