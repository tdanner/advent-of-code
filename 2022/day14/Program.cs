using System.Text.Json;
using static System.Math;

var lines = File.ReadAllLines("input.txt");
var chains = new List<List<Pt>>();
foreach (var line in lines)
{
    var chain = new List<Pt>();
    foreach (var strpt in line.Split(" -> "))
    {
        var comma = strpt.IndexOf(',');
        var pt = new Pt(int.Parse(strpt[..comma]), int.Parse(strpt[(comma + 1)..]));
        chain.Add(pt);
    }
    chains.Add(chain);
}

int minx = int.MaxValue, maxx = int.MinValue, miny = 0, maxy = int.MinValue;
foreach (var pt in chains.SelectMany(c => c))
{
    minx = int.Min(minx, pt.x);
    maxx = int.Max(maxx, pt.x);
    maxy = int.Max(maxy, pt.y);
}

// make room for floor
minx -= 200;
maxx += 200;
maxy += 2;
chains.Add(new List<Pt> { new Pt(minx, maxy), new Pt(maxx, maxy) }); // long floor

int w = maxx - minx, h = maxy;
char[,] field = new char[h + 1, maxx + 2]; // wastes some memory from 0 to minx, but who cares
for (int y = 0; y <= h; y++)
{
    for (int x = minx - 1; x <= maxx + 1; x++)
    {
        field[y, x] = '.';
    }
}

foreach (var chain in chains)
{
    for (int i = 0; i < chain.Count - 1; i++)
    {
        Pt p1 = chain[i], p2 = chain[i + 1];
        int dx = Sign(p2.x - p1.x), dy = Sign(p2.y - p1.y);
        Pt p = p1;
        while (p != p2)
        {
            field[p.y, p.x] = '#';
            p.x += dx;
            p.y += dy;
        }
        field[p.y, p.x] = '#';
    }
}

int sandCount = 0;
bool fallingForever = false;
while (!fallingForever)
{
    // Console.WriteLine();
    // PrintField();
    if (field[0, 500] != '.')
    {
        break; // field is full. no more sand.
    }
    Pt sand = new Pt(500, 0);
    bool TryMove(int dx, int dy)
    {
        if (field[sand.y + dy, sand.x + dx] == '.')
        {
            sand.x += dx;
            sand.y += dy;
            return true;
        }
        return false;
    }
    while (true)
    {
        if (!TryMove(0, 1) && !TryMove(-1, 1) && !TryMove(1, 1))
        {
            sandCount++;
            field[sand.y, sand.x] = 'o';
            break;
        }
        if (sand.y >= maxy)
        {
            Console.WriteLine("floor not wide enough!");
            fallingForever = true;
            break;
        }
    }
}

Console.WriteLine($"Part 1: {sandCount}");

void PrintField()
{
    for (int y = 0; y <= h; y++)
    {
        for (int x = minx; x <= maxx; x++)
        {
            Console.Write(field[y, x]);
        }
        Console.WriteLine();
    }
}

record struct Pt(int x, int y)
{
    public override string ToString() => $"{x},{y}";
}
