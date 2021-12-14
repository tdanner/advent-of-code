using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TimDanner.Utils;

public class Bimap<T1, T2> : IDictionary<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly Dictionary<T1, T2> forward;
    private readonly Dictionary<T2, T1> reverse;

    public Bimap()
    {
        forward = new();
        reverse = new();
    }

    public Bimap(Bimap<T1, T2> other)
    {
        forward = new Dictionary<T1, T2>(other.forward);
        reverse = new Dictionary<T2, T1>(other.reverse);
    }

    private Bimap(Dictionary<T1, T2> forward, Dictionary<T2, T1> reverse)
    {
        this.forward = forward;
        this.reverse = reverse;
    }

    public Bimap<T2, T1> GetReversed()
    {
        return new Bimap<T2, T1>(new Dictionary<T2, T1>(reverse), new Dictionary<T1, T2>(forward));
    }

    public T2 this[T1 key]
    {
        get => forward[key];

        set
        {
            forward[key] = value;
            reverse[value] = key;
        }
    }

    public T1 GetByValue(T2 value)
    {
        if (TryGetKeyFromValue(value, out T1? key))
            return key;
        else
            throw new KeyNotFoundException($"Value {value} not found");
    }

    public ICollection<T1> Keys => forward.Keys;

    public ICollection<T2> Values => forward.Values;

    public int Count => forward.Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<T1, T2>>)forward).IsReadOnly;

    public void Add(T1 key, T2 value)
    {
        forward.Add(key, value);
        reverse.Add(value, key);
    }

    public void Add(KeyValuePair<T1, T2> item)
    {
        forward.Add(item.Key, item.Value);
        reverse.Add(item.Value, item.Key);
    }

    public void Clear()
    {
        forward.Clear();
        reverse.Clear();
    }

    public bool Contains(KeyValuePair<T1, T2> item) => ((ICollection<KeyValuePair<T1, T2>>)forward).Contains(item);
    public bool ContainsKey(T1 key) => forward.ContainsKey(key);
    public bool ContainsValue(T2 value) => reverse.ContainsKey(value);
    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => ((ICollection<KeyValuePair<T1, T2>>)forward).CopyTo(array, arrayIndex);
    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => ((IEnumerable<KeyValuePair<T1, T2>>)forward).GetEnumerator();
    public bool Remove(T1 key)
    {
        if (forward.TryGetValue(key, out T2? value))
        {
            forward.Remove(key);
            reverse.Remove(value);
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<T1, T2> item) => Remove(item.Key);
    public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value) => forward.TryGetValue(key, out value);
    public bool TryGetKeyFromValue(T2 value, [MaybeNullWhen(false)] out T1 key) => reverse.TryGetValue(value, out key);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)forward).GetEnumerator();
}