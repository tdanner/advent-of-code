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


var part1 = cards.Select(c => c.Value()).Sum();
Console.WriteLine($"part 1: {part1}");

var counts = new int[cards.Length];
for (int i = 0; i < counts.Length; i++)
{
    counts[i] = 1;
}

for (int i = 0; i < cards.Length; i++)
{
    int winCount = cards[i].WinCount();
    for (int w = 0; w < winCount; w++)
    {
        counts[i + 1 + w] += counts[i];
    }
}

var part2 = counts.Sum();
Console.WriteLine($"part 2: {part2}");

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

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
