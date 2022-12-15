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

//Console.WriteLine(JsonSerializer.Serialize(sensors, new JsonSerializerOptions { WriteIndented = true }));

var targetY = 2000000;

var noBeacons = new HashSet<int>();
foreach (var sensor in sensors)
{
    var range = sensor.ExclusionAtY(targetY);
    for (int x = range.start; x <= range.end; x++)
    {
        noBeacons.Add(x);
    }
}
foreach (var beacon in beacons)
{
    if (beacon.y == targetY)
    {
        noBeacons.Remove(beacon.x);
    }
}

var part1 = noBeacons.Count;
Console.WriteLine($"Part 1: {part1}");

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
    public bool Contains(int x) => x >= start && x <= end;
}
