using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Math;

// target area: x=70..96, y=-179..-124
string input = File.ReadAllText("input.txt");
// string input = "target area: x=20..30, y=-10..-5";
var match = Regex.Match(input, @"^target area: x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)$");
if (!match.Success) throw new InvalidOperationException("Couldn't parse input");

Rect target = new(new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[4].Value)),
                  new(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)));

var vxs = FindValidXVelocities();
var vys = FindValidYVelocities();
Console.WriteLine($"X({vxs.Count}): {string.Join(",", vxs)}");
Console.WriteLine($"Y({vys.Count}): {string.Join(",", vys)}");

List<Point> valids = new();
foreach (var vx in vxs)
{
    foreach (var vy in vys)
    {
        Point v = new(vx, vy);
        if (PlotTrajectory(v, out _, out _) == Outcome.Hit)
        {
            valids.Add(v);
        }
    }
}

Console.WriteLine(string.Join("\t", valids));
Console.WriteLine($"{valids.Count} velocities found");

Outcome PlotTrajectory(Point velocity, out int maxY, out List<Point> path)
{
    Point position = new(0, 0);
    maxY = 0;
    path = new();
    while (true)
    {
        position = new(position.X + velocity.X, position.Y + velocity.Y);
        path.Add(position);
        maxY = Max(maxY, position.Y);
        velocity = new(Sign(velocity.X) * (Abs(velocity.X) - 1), velocity.Y - 1);

        if (target.Contains(position))
            return Outcome.Hit;
        if (position.Y < target.BottomRight.Y && Abs(velocity.Y) > target.Height + 1)
            return Outcome.TooFast;
        if (position.X > target.BottomRight.X)
            return Outcome.TooFar;
        if (position.Y < target.BottomRight.Y && position.X < target.TopLeft.X)
            return Outcome.TooShort;
    }
}

List<int> FindValidXVelocities()
{
    List<int> valids = new();
    for (int vxStart = 1; vxStart <= target.BottomRight.X; vxStart++)
    {
        int px = 0;
        int vx = vxStart;
        while (px <= target.BottomRight.X)
        {
            px += vx;
            vx = Sign(vx) * (Abs(vx) - 1);
            if (px >= target.TopLeft.X && px <= target.BottomRight.X)
            {
                valids.Add(vxStart);
                break;
            }
            if (vx == 0) break;
        }
    }
    return valids;
}

List<int> FindValidYVelocities()
{
    List<int> valids = new();
    for (int vyStart = target.BottomRight.Y; vyStart < 10000; vyStart++)
    {
        int py = 0;
        int vy = vyStart;
        while (py >= target.BottomRight.Y)
        {
            py += vy;
            vy--;
            if (py <= target.TopLeft.Y && py >= target.BottomRight.Y)
            {
                valids.Add(vyStart);
                break;
            }
        }
    }
    return valids;
}

[DebuggerDisplay("{X},{Y}")]
record Point(int X, int Y)
{
    public override string ToString() => $"{X},{Y}";
}

[DebuggerDisplay("{TopLeft}:{BottomRight}")]
record Rect(Point TopLeft, Point BottomRight)
{
    public bool Contains(Point p) => p.X >= TopLeft.X && p.Y <= TopLeft.Y &&
                                     p.X <= BottomRight.X && p.Y >= BottomRight.Y;
    public int Width => BottomRight.X - TopLeft.X;
    public int Height => TopLeft.Y - BottomRight.Y;
}

enum Outcome
{
    Hit,        // entered target region
    TooShort,   // Y below before X entered range
    TooFar,     // X beyond before Y entered range
    TooFast,    // X and Y exceeded range in the same step
}
