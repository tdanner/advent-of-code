using System.Text.Json;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input1.txt");
var re = new Regex(@"^Card +(\d+):(?: +(\d+))+ \|(?: +(\d+))+$");

var cards = lines.Select(l => re.Match(l)).Select(m => new Card
(
    int.Parse(m.Groups[1].Value),
    m.Groups[2].Captures.Select(c => int.Parse(c.Value)).ToArray(),
    m.Groups[3].Captures.Select(c => int.Parse(c.Value)).ToArray()
)).ToArray();

foreach (var c in cards)
{
    Console.WriteLine($"{c} - {c.WinCount()} winning; {c.Value()} points");
}

foreach (var c in cards.Where(c => c.Duplicates()))
{
    Console.WriteLine(c);
}

var part1 = cards.Select(c => c.Value()).Sum();
Console.WriteLine($"part 1: {part1}");

public record class Card(int Num, int[] Winning, int[] Have)
{
    public int WinCount()
    {
        return Winning.Intersect(Have).Count();
    }

    public int Value()
    {
        int w = WinCount();
        if (w == 0) { return 0; }
        return 1 << (w - 1);
    }

    public bool Duplicates()
    {
        return new HashSet<int>(Have).Count < Have.Length ||
            new HashSet<int>(Winning).Count < Winning.Length;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
