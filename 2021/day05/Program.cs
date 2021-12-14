using System.Text.RegularExpressions;
using static System.Math;

record Point(int x, int y)
{

}

record Line(Point start, Point end)
{
    public IEnumerable<Point> GridPoints()
    {
        int xDir = Sign(end.x - start.x);
        int yDir = Sign(end.y - start.y);
        int numPoints = Max(Abs(end.x - start.x), Abs(end.y - start.y)) + 1;
        for (int pos = 0; pos < numPoints; ++pos)
        {
            yield return new Point(start.x + pos*xDir, start.y + pos*yDir);
        }
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
