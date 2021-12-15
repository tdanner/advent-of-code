namespace TimDanner.Utils;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action(item);
        }
    }

    public static T? MinBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        where TKey : IComparable
    {
        T? first = items.FirstOrDefault();
        if (first == null)
        {
            return first;
        }

        T min = first;
        TKey minValue = selector(first);

        foreach (var item in items)
        {
            TKey value = selector(item);
            if (value.CompareTo(minValue) < 0)
            {
                minValue = value;
                min = item;
            }
        }

        return min;
    }
}
