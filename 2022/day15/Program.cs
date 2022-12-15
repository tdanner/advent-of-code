using System.Collections;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Math;

var lines = File.ReadAllLines("input.txt");
var parser = new Regex(@"Sensor at x=([-\d]+), y=([-\d]+): closest beacon is at x=([-\d]+), y=([-\d]+)");
var sensors = new List<Sensor>();
var beacons = new List<Pt>();
foreach (var line in lines)
{
    var m = parser.Match(line);
    var s = new Sensor();
    s.Loc = new Pt(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
    var b = new Pt(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
    beacons.Add(b);
    s.ExclusionDist = Manhattan(s.Loc, b);
    sensors.Add(s);
}
const long h = 4000000;
const long w = 4000000;
for (int y = 0; y <= h; y++)
{
    var possible = new RangeSet();
    foreach (var sensor in sensors)
    {
        possible.Include(sensor.ExclusionAtY(y));
    }
    var candidates = possible.Excluded(new Range(0, (int)w)).ToList();
    if (candidates.Count > 0)
    {
        Console.WriteLine($"count {candidates.Count} {JsonSerializer.Serialize(candidates)} at y {y}");
        long freq = w * candidates.Single() + (long)y;
        Console.WriteLine($"Part 2: {freq}");
    }
}

int Manhattan(Pt a, Pt b) => Abs(a.x - b.x) + Abs(a.y - b.y);

class Sensor
{
    public Pt Loc { get; set; }
    public int ExclusionDist { get; set; }

    public Range ExclusionAtY(int y)
    {
        int w = ExclusionDist - Abs(Loc.y - y);
        if (w < 0)
        {
            return Range.Empty;
        }
        return new Range(Loc.x - w, Loc.x + w);
    }
}

record struct Pt(int x, int y)
{
    public override string ToString() => $"{x},{y}";
}

record class Range(int start, int end)
{
    public static Range Empty = new Range(1, 0);
}

class RangeSet
{
    List<Range> included = new();

    public void Include(Range r)
    {
        if (r != Range.Empty)
        {
            included.Add(r);
        }
    }

    public IEnumerable<int> Excluded(Range toScan)
    {
        for (int x = toScan.start; x <= toScan.end; x++)
        {
            bool inRange = true;
            foreach (var include in included)
            {
                if (x >= include.start && x <= include.end)
                {
                    x = Max(x, include.end);
                    inRange = false;
                    break;
                }
            }
            if (inRange)
            {
                yield return x;
            }
        }
    }
}
