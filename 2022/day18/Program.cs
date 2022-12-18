internal class Program
{
    private static void Main(string[] args)
    {
        var pts = File.ReadAllLines("input.txt")
              .Select(s => s.Split(','))
              .Select(s => new Pt(int.Parse(s[0]) + 1, int.Parse(s[1]) + 1, int.Parse(s[2]) + 1))
              .ToList();
        var rx = new Range(pts.Select(pt => pt.x).Min(), pts.Select(pt => pt.x).Max());
        var ry = new Range(pts.Select(pt => pt.y).Min(), pts.Select(pt => pt.y).Max());
        var rz = new Range(pts.Select(pt => pt.z).Min(), pts.Select(pt => pt.z).Max());

        bool[,,] vol = new bool[rx.max + 2, ry.max + 2, rz.max + 2];
        foreach (var pt in pts)
        {
            vol[pt.x, pt.y, pt.z] = true;
        }

        Console.WriteLine($"to scan: {(rx.max + 3) * (ry.max + 3) * (rz.max + 3)}");

        int faces = 0;
        for (int x = 1; x <= rx.max; x++)
        {
            for (int y = 1; y <= ry.max; y++)
            {
                for (int z = 1; z <= rz.max; z++)
                {
                    if (!vol[x, y, z])
                    {
                        continue;
                    }
                    // count which of the six faces are exposed
                    var count = (int dx, int dy, int dz) => faces += vol[x + dx, y + dy, z + dz] ? 0 : 1;
                    count(-1, 0, 0);
                    count(1, 0, 0);
                    count(0, -1, 0);
                    count(0, 1, 0);
                    count(0, 0, -1);
                    count(0, 0, 1);
                }
            }
        }
        Console.WriteLine($"Part 1: {faces}");

        // Part 2 - 
        //   Make another volume array called ext and "flood fill" it from the outside
        //   Count the faces in vol that are touching a cube in ext

        bool[,,] ext = new bool[rx.max + 2, ry.max + 2, rz.max + 2];
        var toPaint = new Queue<Pt>();
        toPaint.Enqueue(new Pt(0, 0, 0));
        while (toPaint.Any())
        {
            var consider = (Pt p) =>
            {
                if (p.InRange(ext) && ext[p.x, p.y, p.z] == false && vol[p.x, p.y, p.z] == false)
                {
                    ext[p.x, p.y, p.z] = true;
                    toPaint.Enqueue(p);
                }
            };
            foreach (var n in toPaint.Dequeue().Neighbors())
            {
                consider(n);
            }
        }

        faces = 0;
        for (int x = 1; x <= rx.max; x++)
        {
            for (int y = 1; y <= ry.max; y++)
            {
                for (int z = 1; z <= rz.max; z++)
                {
                    if (!vol[x, y, z])
                    {
                        continue;
                    }
                    // count which of the six faces are exposed
                    var count = (int dx, int dy, int dz) => faces += ext[x + dx, y + dy, z + dz] ? 1 : 0;
                    count(-1, 0, 0);
                    count(1, 0, 0);
                    count(0, -1, 0);
                    count(0, 1, 0);
                    count(0, 0, -1);
                    count(0, 0, 1);
                }
            }
        }
        Console.WriteLine($"Part 2: {faces}");

    }
}

record struct Range(int min, int max);
record struct Pt(int x, int y, int z)
{
    public bool InRange(bool[,,] space) =>
        x >= 0 && y >= 0 && z >= 0 && x < space.GetLength(0) && y < space.GetLength(1) && z < space.GetLength(2);

    public IEnumerable<Pt> Neighbors()
    {
        yield return this with { x = x - 1 };
        yield return this with { x = x + 1 };
        yield return this with { y = y - 1 };
        yield return this with { y = y + 1 };
        yield return this with { z = z - 1 };
        yield return this with { z = z + 1 };
    }
}
