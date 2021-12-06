using System.Text.RegularExpressions;
using static System.Math;

record Point(int x, int y)
{

}

record Line(Point start, Point end)
{
    public bool IsGridAligned()
    {
        return start.x == end.x || start.y == end.y;
    }

    public IEnumerable<Point> GridPoints()
    {
        if (start.x == end.x)
        {
            for (int y = Min(start.y, end.y); y <= Max(start.y, end.y); ++y)
            {
                yield return new Point(start.x, y);
            }

            yield break;
        }

        if (start.y == end.y)
        {
            for (int x = Min(start.x, end.x); x <= Max(start.x, end.x); ++x)
            {
                yield return new Point(x, start.y);
            }

            yield break;
        }

        // if not grid-aligned, fall through
    }
}

class Program
{
    static void Main()
    {
        const int gridSize = 1000;
        string[] lines = File.ReadAllLines("input.txt");

        var ventLines = lines.Select(line => Regex.Match(line, @"^(\d+),(\d+) -> (\d+),(\d+)$"))
            .Select(m => new Line(new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)), 
                                  new Point(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value))))
            .ToArray();
        
        int gridAlignedLines = ventLines.Count(line => line.IsGridAligned());
        Console.WriteLine($"{gridAlignedLines} grid-aligned lines found");

        int[,] grid = new int[gridSize,gridSize];

        foreach (Line ventLine in ventLines)
        {
            foreach (Point pt in ventLine.GridPoints())
            {
                grid[pt.x, pt.y]++;
            }
        }

        int dangerousPointCount = 0;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y] >= 2)
                    dangerousPointCount++;
            }
        }

        Console.WriteLine(new { dangerousPointCount });
    }
}
