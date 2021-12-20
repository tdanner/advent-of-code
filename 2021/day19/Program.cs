using System.Diagnostics.CodeAnalysis;

static class Program
{
    static void Main()
    {
        List<List<int[]>> scanners = new();
        List<int[]> scanner = new();
        foreach (string line in File.ReadLines("input.txt"))
        {
            if (line.StartsWith("---"))
            {
                scanner = new();
            }
            else if (line == "")
            {
                scanners.Add(scanner);
            }
            else
            {
                int[] coords = line.Split(',').Select(int.Parse).ToArray();
                scanner.Add(coords);
            }
        }
        scanners.Add(scanner);

        Dictionary<(int, int), Orientation> mappings = new();

        List<int[]> beacons = scanners[0];
        List<List<int[]>> unmatchedScanners = scanners.Skip(1).ToList();
        List<int[]> scannerLocations = new();
        while (unmatchedScanners.Any())
        {
            bool matched = false;
            foreach (var s in unmatchedScanners)
            {
                Orientation? o = FindAlignment(beacons, s);
                if (o != null)
                {
                    scannerLocations.Add(o.d);
                    Console.WriteLine(o);
                    beacons = beacons.Union(Adjust(s, o), PointComparer.Instance).ToList();
                    unmatchedScanners.Remove(s);
                    matched = true;
                    break;
                }
            }
            if (!matched) throw new Exception("Failed to match one");
            Console.WriteLine(unmatchedScanners.Count);
        }
        Console.WriteLine(new { beaconCount = beacons.Count });

        int maxDist = 0;
        foreach (var p1 in scannerLocations)
        {
            foreach (var p2 in scannerLocations)
            {
                maxDist = Math.Max(maxDist, Manhattan(p1, p2));
            }
        }
        Console.WriteLine(new { maxDist });

        // for (int s1 = 0; s1 < scanners.Count; s1++)
        // {
        //     for (int s2 = s1 + 1; s2 < scanners.Count; s2++)
        //     {
        //         Orientation? o = FindAlignment(scanners[s1], scanners[s2]);
        //         if (o != null)
        //         {
        //             mappings[(s1, s2)] = o;
        //             Console.WriteLine($"{s1} overlaps {s2}");
        //         }
        //     }
        // }
    }

    static int Manhattan(int[] p1, int[] p2)
    {
        return (p1[0] - p2[0]) + (p1[1] - p2[1]) + (p1[2] - p2[2]);
    }

    static Orientation? FindAlignment(List<int[]> scanner1, List<int[]> scanner2)
    {
        foreach (int[,] r in rotations)
        {
            foreach (int[] anchor1 in scanner1)
            {
                foreach (int[] anchor2 in scanner2)
                {
                    // Compute rotation and translation that aligns these two points
                    int[] anchor2Rotated = Rotate(anchor2, r);
                    int[] translation = new int[3];
                    for (int i = 0; i < 3; i++)
                    {
                        translation[i] = anchor1[i] - anchor2Rotated[i];
                    }

                    // See if there are 12 or more matches using this rotation and translation
                    var orientation = new Orientation(r, translation);
                    var scanner2Adjusted = Adjust(scanner2, orientation);
                    int matches = CountMatches(scanner1, scanner2Adjusted);
                    if (matches >= 12)
                    {
                        return orientation;
                    }
                }
            }
        }

        return null;
    }

    private static int CountMatches(List<int[]> points1, List<int[]> points2)
    {
        int matches = 0;
        foreach (var p1 in points1)
        {
            foreach (var p2 in points2)
            {
                if (p1[0] == p2[0] && p1[1] == p2[1] && p1[2] == p2[2])
                    matches++;
            }
        }
        return matches;
    }

    static List<int[]> Adjust(List<int[]> points, Orientation orientation)
    {
        return points.Select(p => Translate(Rotate(p, orientation.rotation), orientation.d)).ToList();
    }

    static int[] Translate(int[] p, int[] d)
    {
        int[] t = new int[3];
        for (int i = 0; i < 3; i++)
        {
            t[i] = p[i] + d[i];
        }

        return t;
    }

    static int[] Rotate(int[] p, int[,] rotation)
    {
        int[] r = new int[3];
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; j++)
            {
                r[i] += p[j] * rotation[i, j];
            }
        }
        return r;
    }

    static readonly List<int[,]> rotations = new()
    {
        // Facing +x
        new[,] {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 },
            },
        new[,] {
                { 1, 0, 0 },
                { 0, 0, -1 },
                { 0, 1, 0 },
            },
        new[,] {
                { 1, 0, 0 },
                { 0, -1, 0 },
                { 0, 0, -1 },
            },
        new[,] {
                { 1, 0, 0 },
                { 0, 0, 1 },
                { 0, -1, 0 },
            },

        // Facing +y
        new[,] {
                { 0, 1, 0 },
                { -1, 0, 0 },
                { 0, 0, 1 },
            },
        new[,] {
                { 0, 1, 0 },
                { 0, 0, 1 },
                { 1, 0, 0 },
            },
        new[,] {
                { 0, 1, 0 },
                { 1, 0, 0 },
                { 0, 0, -1 },
            },
        new[,] {
                { 0, 1, 0 },
                { 0, 0, -1 },
                { -1, 0, 0 },
            },

        // Facing +z
        new[,] {
                { 0, 0, 1 },
                { 0, -1, 0 },
                { 1, 0, 0 },
            },
        new[,] {
                { 0, 0, 1 },
                { -1, 0, 0 },
                { 0, -1, 0 },
            },
        new[,] {
                { 0, 0, 1 },
                { 0, 1, 0 },
                { -1, 0, 0 },
            },
        new[,] {
                { 0, 0, 1 },
                { 1, 0, 0 },
                { 0, 1, 0 },
            },

        // Facing -x
        new[,] {
                { -1, 0, 0 },
                { 0, -1, 0 },
                { 0, 0, 1 },
            },
        new[,] {
                { -1, 0, 0 },
                { 0, 0, 1 },
                { 0, 1, 0 },
            },
        new[,] {
                { -1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, -1 },
            },
        new[,] {
                { -1, 0, 0 },
                { 0, 0, -1 },
                { 0, -1, 0 },
            },

        // Facing -y
        new[,] {
                { 0, -1, 0 },
                { 1, 0, 0 },
                { 0, 0, 1 },
            },
        new[,] {
                { 0, -1, 0 },
                { 0, 0, 1 },
                { -1, 0, 0 },
            },
        new[,] {
                { 0, -1, 0 },
                { -1, 0, 0 },
                { 0, 0, -1 },
            },
        new[,] {
                { 0, -1, 0 },
                { 0, 0, -1 },
                { 1, 0, 0 },
            },

        // Facing -z
        new[,] {
                { 0, 0, -1 },
                { 1, 0, 0 },
                { 0, -1, 0 },
            },
        new[,] {
                { 0, 0, -1 },
                { 0, 1, 0 },
                { 1, 0, 0 },
            },
        new[,] {
                { 0, 0, -1 },
                { -1, 0, 0 },
                { 0, 1, 0 },
            },
        new[,] {
                { 0, 0, -1 },
                { 0, -1, 0 },
                { -1, 0, 0 },
            },
    };
}

internal class PointComparer : IEqualityComparer<int[]>
{
    public static readonly PointComparer Instance = new();

    public bool Equals(int[]? x, int[]? y)
    {
        if (x == y) return true;
        if (x == null || y == null) return false;
        if (x.Length != y.Length) return false;
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    public int GetHashCode([DisallowNull] int[] obj)
    {
        unchecked
        {
            int hash = 17;
            foreach (var item in obj)
            {
                hash = hash * 31 + item.GetHashCode();
            }
            return hash;
        }
    }
}

record Orientation(int[,] rotation, int[] d)
{
    public override string ToString()
    {
        return $@"
Rotation:
{rotation[0, 0]} {rotation[0, 1]} {rotation[0, 2]} 
{rotation[1, 0]} {rotation[1, 1]} {rotation[1, 2]} 
{rotation[2, 0]} {rotation[2, 1]} {rotation[2, 2]} 
Translation: {d[0]},{d[1]},{d[2]}
";
    }
}
