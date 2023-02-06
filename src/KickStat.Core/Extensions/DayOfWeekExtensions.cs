namespace KickStat;

public static class DayOfWeekExtensions
{
    // Считаем от 0
    private static int TOTAL_DAYS_COUNT = 6;
    
    public static int GetIndex(this DayOfWeek source)
    {
        return source == DayOfWeek.Sunday ? TOTAL_DAYS_COUNT : (int) source - 1;
    }

    public static int ClosestDistance(this DayOfWeek[] source, DayOfWeek current)
    {
        if (!source.Any())
            return 0;

        var currentIndex = GetIndex(current);
        source = source.OrderBy(x => x).ToArray();

        foreach (var dayOfWeek in source)
        {
            var index = dayOfWeek.GetIndex();

            if (currentIndex == index)
                return 0;

            if (index > currentIndex)
                return index - currentIndex;
        }

        var firstIndex = source.First().GetIndex();
        return Math.Abs((TOTAL_DAYS_COUNT + 1 - currentIndex) + firstIndex);
    }

    public static DayOfWeek? Closest(this DayOfWeek[] source, DayOfWeek current)
    {
        if (!source.Any())
            return null;

        var currentIndex = GetIndex(current);
        source = source.OrderBy(x => x).ToArray();

        foreach (var dayOfWeek in source)
        {
            var index = dayOfWeek.GetIndex();

            if (currentIndex == index)
                return current;

            if (index > currentIndex)
                return GetByIndex(index);
        }

        return source.First();

    }
    
    private static DayOfWeek GetByIndex(int value)
    {
        if (value == TOTAL_DAYS_COUNT)
            return DayOfWeek.Sunday;
        else
            return (DayOfWeek) (value + 1);
    }


}