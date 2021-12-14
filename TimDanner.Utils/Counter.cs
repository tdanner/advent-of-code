using System.Collections;

namespace TimDanner.Utils;

public class Counter<T> : IEnumerable<KeyValuePair<T, long>>
    where T : notnull
{
    private readonly Dictionary<T, long> counts = new();

    public long this[T key]
    {
        get => counts.TryGetValue(key, out long count) ? count : 0;
        set => counts[key] = value;
    }

    public IEnumerable<T> Keys
    {
        get => counts.Keys;
    }

    public IEnumerable<long> Values
    {
        get => counts.Values;
    }

    public IEnumerator<KeyValuePair<T, long>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<T, long>>)counts).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)counts).GetEnumerator();
    }
}
