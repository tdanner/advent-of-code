using System.Text.Json;

var lines = File.ReadAllLines("input.txt");

int part1 = 0;
for (int pair = 1; pair <= (lines.Length + 1) / 3; pair++)
{
    var left = Parse(lines[(pair - 1) * 3]);
    var right = Parse(lines[(pair - 1) * 3 + 1]);
    Console.WriteLine(JsonSerializer.Serialize(left));
    Console.WriteLine(JsonSerializer.Serialize(right));
    int cmp = Compare(left, right);
    Console.WriteLine($"Pair {pair} is in the " + cmp switch
    {
        -1 => "right",
        0 => "same",
        1 => "wrong",
        _ => "wtf"
    } + " order");
    if (cmp == -1)
    {
        part1 += pair;
    }
}

Console.WriteLine($"Part 1: {part1}");

int Compare(object left, object right)
{
    if (left is int a && right is int b)
    {
        return a.CompareTo(b);
    }
    if (left is List<object> c && right is List<object> d)
    {
        for (int i = 0; i < Math.Min(c.Count, d.Count); i++)
        {
            int cmp = Compare(c[i], d[i]);
            if (cmp != 0)
            {
                return cmp;
            }
        }
        return c.Count.CompareTo(d.Count);
    }
    if (left is int e)
    {
        return Compare(new List<object> { e }, right);
    }
    if (right is int f)
    {
        return Compare(left, new List<object> { f });
    }
    throw new Exception("wtf");
}

object Parse(string line)
{
    var reader = new StringReader(line);
    return ReadValue(reader);
}

object ReadValue(StringReader reader)
{
    int read = reader.Read();
    if (read == -1)
    {
        throw new Exception("nothing here");
    }
    char c = (char)read;
    if (char.IsDigit(c))
    {
        string i = new string(c, 1);
        while (char.IsDigit((char)reader.Peek()))
        {
            i += (char)reader.Read();
        }
        return int.Parse(i);
    }
    else if (c == '[')
    {
        var l = new List<object>();
        while ((char)reader.Peek() != ']')
        {
            l.Add(ReadValue(reader));
            if ((char)reader.Peek() == ',')
            {
                reader.Read(); // consume comma
            }
        }
        reader.Read(); // consume ]
        return l;
    }
    else
    {
        throw new Exception($"can't parse {c}");
    }
}
