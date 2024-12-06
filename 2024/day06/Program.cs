var lines = File.ReadAllLines("input.txt");
int width = lines[0].Length;
int height = lines.Length;

Point guard = Point.Zero;
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        if (lines[y][x] == '^')
        {
            guard = new Point(x, y);
        }
    }
}
var direction = new Point(0, -1);

HashSet<Point> visited = [];
while (true)
{
    visited.Add(guard);
    var next = guard.Add(direction);
    if (!InBounds(next))
    {
        break;
    }
    if (Obstacle(next))
    {
        direction = TurnRight(direction);
        next = guard.Add(direction);
    }
    guard = next;
}
Console.WriteLine($"Part 1: {visited.Count}");

bool InBounds(Point p)
{
    return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
}

bool Obstacle(Point p)
{
    return lines[p.y][p.x] == '#';
}

Point TurnRight(Point dir)
{
    return dir switch
    {
        Point(0, -1) => new Point(1, 0),
        Point(1, 0) => new Point(0, 1),
        Point(0, 1) => new Point(-1, 0),
        Point(-1, 0) => new Point(0, -1),
        _ => throw new Exception("invalid direction")
    };
}

record struct Point(int x, int y)
{
    public static readonly Point Zero = new(0, 0);
    public readonly Point Add(Point other)
    {
        return new Point(x + other.x, y + other.y);
    }
}
