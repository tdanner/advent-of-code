using System.Diagnostics;

internal class Program
{
    private static void Main()
    {
        string[] lines = File.ReadAllLines("input.txt");
        int tileSize = lines.Length;
        Debug.Assert(lines[0].Length == tileSize);
        int[,] tile = new int[tileSize, tileSize];
        int size = tileSize * 5;
        int[,] map = new int[size, size];
        for (int y = 0; y < tileSize; y++)
        {
            for (int x = 0; x < tileSize; x++)
            {
                tile[y, x] = lines[y][x] - '0';
            }
        }

        for (int repeatY = 0; repeatY < 5; repeatY++)
        {
            for (int repeatX = 0; repeatX < 5; repeatX++)
            {
                for (int y = 0; y < tileSize; y++)
                {
                    for (int x = 0; x < tileSize; x++)
                    {
                        map[y + (tileSize * repeatY), x + (tileSize * repeatX)] =
                            ((tile[y, x] + repeatX + repeatY - 1) % 9) + 1;
                    }
                }
            }
        }

        PriorityQueue<Point, int> q = new();
        Dictionary<Point, int> dist = new();
        //Dictionary<Point, Point?> prev = new();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                var p = new Point(x, y);
                dist[p] = int.MaxValue;
                //prev[p] = null;
            }
        }

        dist[new Point(0, 0)] = 0;
        q.Enqueue(new Point(0, 0), 0);

        IEnumerable<Point> Neighbors(Point p)
        {
            if (p.Y > 0)
            {
                yield return new Point(p.X, p.Y - 1);
            }

            if (p.X < map.GetLength(1) - 1)
            {
                yield return new Point(p.X + 1, p.Y);
            }

            if (p.Y < map.GetLength(0) - 1)
            {
                yield return new Point(p.X, p.Y + 1);
            }

            if (p.X > 0)
            {
                yield return new Point(p.X - 1, p.Y);
            }
        }

        // Dijkstra!
        HashSet<Point> visited = new();
        var watch = Stopwatch.StartNew();
        while (q.TryDequeue(out Point? u, out _))
        {
            if (visited.Contains(u))
            {
                continue;
            }

            visited.Add(u);

            foreach (Point v in Neighbors(u).Where(n => dist[n] == int.MaxValue))
            {
                int alt = dist[u] + map[v.Y, v.X];
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    q.Enqueue(v, alt);
                    //prev[v] = u;
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
        Console.WriteLine(new { lowestTotalRisk = dist[new Point(map.GetLength(1) - 1, map.GetLength(0) - 1)] });
    }
}

internal record Point(int X, int Y);
