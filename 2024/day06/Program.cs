var lines = File.ReadAllLines("input.txt");
int width = lines[0].Length;
int height = lines.Length;

Point starting = Point.Zero;
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        if (lines[y][x] == '^')
        {
            starting = new Point(x, y);
        }
    }
}
var guard = starting;
var startingDir = new Point(0, -1);
var direction = startingDir;

HashSet<Point> visited = [];
HashSet<Point> options = [];
while (true)
{
    visited.Add(guard);
    var next = guard.Add(direction);
    if (!InBounds(next))
    {
        break;
    }
    while (Obstacle(next))
    {
        direction = TurnRight(direction);
        next = guard.Add(direction);
    }
    guard = next;
}
Console.WriteLine($"Part 1: {visited.Count}");

foreach (var extraObstacle in visited)
{
    if (CreatesLoop(starting, extraObstacle, startingDir))
    {
        options.Add(extraObstacle);
    }
}

Console.WriteLine($"Part 2: {options.Count}");
Console.WriteLine($"options includes starting: {options.Contains(starting)}");
foreach (var option in options)
{
    if (Obstacle(option))
    {
        Console.WriteLine($"{option} is both an option and an obstacle");
    }
    if (!InBounds(option))
    {
        Console.WriteLine($"{option} is out of bounds");
    }
}

// for (int y = 0; y < height; y++)
// {
//     for (int x = 0; x < width; x++)
//     {
//         if (options.Contains(new Point(x, y)))
//         {
//             Console.Write('O');
//         }
//         else
//         {
//             Console.Write(lines[y][x]);
//         }
//     }
//     Console.WriteLine();
// }

bool CreatesLoop(Point startingPos, Point extraObstacle, Point startingDir)
{
    var guard = startingPos;
    var dir = startingDir;
    HashSet<(Point, Point)> turns = [];
    while (true)
    {
        var next = guard.Add(dir);
        while (Obstacle(next) || next == extraObstacle)
        {
            if (!turns.Add((guard, dir)))
            {
                return true;
            }
            dir = TurnRight(dir);
            next = guard.Add(dir);
        }
        if (!InBounds(next))
        {
            return false;
        }
        guard = next;
    }
}

bool InBounds(Point p)
{
    return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
}

bool Obstacle(Point p)
{
    return InBounds(p) && lines[p.y][p.x] == '#';
}

static Point TurnRight(Point dir)
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
