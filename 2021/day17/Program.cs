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

int maxY = int.MinValue;
Point velocity = new(1, 1);
int attempts = 1000;
int velocityYLimit = int.MaxValue;
while (--attempts > 0 && velocity.Y < velocityYLimit)
{
    Outcome outcome = PlotTrajectory(velocity, out int peak, out var path);

    Console.WriteLine($"{velocity} {outcome}");
    switch (outcome)
    {
        case Outcome.Hit:
            if (peak > maxY) Console.WriteLine($"{velocity} {peak}");
            maxY = Max(maxY, peak);
            velocity = new(velocity.X, velocity.Y + 1);
            break;
        case Outcome.TooFast:
            // PrintPath(path, outcome);
            velocityYLimit = velocity.Y * 3;
            velocity = new(velocity.X, velocity.Y + 1);
            Console.WriteLine(new { maxY });
            break;
        case Outcome.TooFar:
            velocity = new(velocity.X - 1, velocity.Y);
            break;
        case Outcome.TooShort:
            velocity = new(velocity.X + 1, velocity.Y);
            break;
    }
}

void PrintPath(List<Point> path, Outcome outcome)
{
    Rect bounding = new(new(0, path.TakeLast(2).Append(target.TopLeft).Select(p => p.Y).Max()),
    new(path.TakeLast(2).Append(target.BottomRight).Select(p => p.X).Max(),
        path.TakeLast(2).Append(target.BottomRight).Select(p => p.Y).Min()));
    Console.WriteLine();
    Console.WriteLine(outcome);
    for (int y = bounding.TopLeft.Y; y >= bounding.BottomRight.Y; y--)
    {
        for (int x = bounding.TopLeft.X; x <= bounding.BottomRight.X; x++)
        {
            if (x == 0 && y == 0)
                Console.Write("S");
            else if (path.Contains(new(x, y)))
                Console.Write('#');
            else if (target.Contains(new(x, y)))
                Console.Write('T');
            else
                Console.Write('.');
        }
        Console.WriteLine();
    }
}

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

[DebuggerDisplay("{X},{Y}")]
record Point(int X, int Y);

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
