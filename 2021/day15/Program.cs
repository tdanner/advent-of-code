class Program
{
    static void Main()
    {
        var lines = File.ReadAllLines("input.txt");
        int[,] map = new int[lines.Length, lines[0].Length];
        List<Point> q = new();
        Dictionary<Point, int> dist = new();
        Dictionary<Point, Point?> prev = new();
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[0].Length; x++)
            {
                var p = new Point(x, y);
                q.Add(p);
                dist[p] = int.MaxValue;
                prev[p] = null;
                map[y, x] = lines[y][x] - '0';
            }
        }
        dist[new Point(0, 0)] = 0;

        IEnumerable<Point> Neighbors(Point p)
        {
            if (p.y > 0) yield return new Point(p.x, p.y - 1);
            if (p.x < map.GetLength(1) - 1) yield return new Point(p.x + 1, p.y);
            if (p.y < map.GetLength(0) - 1) yield return new Point(p.x, p.y + 1);
            if (p.x > 0) yield return new Point(p.x - 1, p.y);
        }

        // Dijkstra!
        while (q.Any())
        {
            Point u = q.MinBy(p => dist[p])!;
            q.Remove(u);
            foreach (Point v in Neighbors(u).Where(q.Contains))
            {
                int alt = dist[u] + map[v.y, v.x];
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        // List<Point> path = new();
        // Point? here = new(map.GetLength(1) - 1, map.GetLength(0) - 1);
        // while (here != null)
        // {
        //     path.Add(here);
        //     here = prev[here];
        // }
        // Console.WriteLine(string.Join(',', path));
        Console.WriteLine(new
        {
            lowestTotalRisk = dist[new Point(map.GetLength(1) - 1, map.GetLength(0) - 1)]
        });
    }
}

record Point(int x, int y);