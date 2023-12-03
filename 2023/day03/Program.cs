using System.Text.RegularExpressions;
using static System.Math;

var lines = File.ReadAllLines("input1.txt");
var sym = new Regex(@"[^\.0-9]");
var num = new Regex(@"[0-9]+");
var parts = new List<Part>();
var map = new bool[lines[0].Length, lines.Length];
for (int y = 0; y < lines.Length; y++)
{
    parts.AddRange(num.Matches(lines[y]).Cast<Match>().Select(n =>
        new Part
        {
            num = int.Parse(n.Value),
            y = y,
            x1 = n.Index,
            x2 = n.Index + n.Length - 1
        }));
    foreach (var m in sym.Matches(lines[y]).Cast<Match>())
        map[m.Index, y] = true;
}

// foreach (var p in parts)
//     Console.WriteLine($"{p.num} at {p.x1}-{p.x2},{p.y}");

int total = 0;
foreach (var p in parts)
{
    bool ok = false;
    for (int y = Max(0, p.y - 1); y <= Min(lines.Length - 1, p.y + 1); y++)
    {
        for (int x = Max(0, p.x1 - 1); x <= Min(lines[0].Length - 1, p.x2 + 1); x++)
        {
            ok |= map[x, y];
        }
    }
    if (ok)
    {
        total += p.num;
    }
    else
    {
        Console.WriteLine($"Not ok {p.num}");
    }
}

Console.WriteLine(total);

class Part
{
    public int num;
    public int x1, x2, y;
}
