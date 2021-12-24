using System.Diagnostics;
using System.Text.RegularExpressions;

List<(bool on, Cube cube)> steps = new();
foreach (string line in File.ReadAllLines("simple.txt"))
{
    var match = Regex.Match(line, @"^(on|off) x=(-?\d+)..(-?\d+),y=(-?\d+)..(-?\d+),z=(-?\d+)..(-?\d+)$");
    Debug.Assert(match.Success);
    steps.Add(
        (match.Groups[1].Value == "on",
         new(int.Parse(match.Groups[2].Value),
             int.Parse(match.Groups[3].Value) + 1,
             int.Parse(match.Groups[4].Value),
             int.Parse(match.Groups[5].Value) + 1,
             int.Parse(match.Groups[6].Value),
             int.Parse(match.Groups[7].Value) + 1)));
    Console.WriteLine(steps[^1]);
}



List<Cube> onCubes = new();
for (int stepNum = 0; stepNum < steps.Count; stepNum++)
{
    var step = steps[stepNum];

    List<Cube> onCubes2 = new();
    foreach (var onCube in onCubes)
    {
        if (Intersect(step.cube, onCube, out Cube? intersection, out List<Cube> leftRemainder, out List<Cube> rightRemainder))
        {
            onCubes2.AddRange(rightRemainder);
            if (step.on)
            {
                onCubes2.Add(intersection!);
                onCubes2.AddRange(leftRemainder);
            }
        }
        else
        {
            onCubes2.Add(step.cube);
        }
    }

    if (stepNum == 0)
    {
        onCubes2.Add(step.cube);
    }

    onCubes = onCubes2;

    Console.WriteLine();
    Console.WriteLine($"After step {stepNum + 1} volume = {onCubes.Sum(c => c.Volume())}");
    foreach (var point in onCubes.SelectMany(cube => cube.Points()).OrderBy(p => p))
    {
        Console.WriteLine(point);
    }
}

Console.WriteLine(onCubes.Sum(cube => cube.Volume()));

bool Intersect(Cube left, Cube right, out Cube? intersection, out List<Cube> leftRemainder, out List<Cube> rightRemainder)
{
    // returns true if left and right overlap
    // intersection gets the overlap
    // leftRemainder gets a set of cubes corresponding to all parts of left minus right
    // rightRemainder gets a set of cubes corresponding to all parts of right minus left
    // no attempt to minimize the number of output cubes

    intersection = null;
    leftRemainder = new();
    rightRemainder = new();

    if (left.Xmin >= right.Xmax || left.Xmax <= right.Xmin ||
        left.Ymin >= right.Ymax || left.Ymax <= right.Ymin ||
        left.Zmin >= right.Zmax || left.Zmax <= right.Zmin)
        return false;

    // some overlap - slice the space into 27 subcubes!
    List<int> x = new[] { left.Xmin, right.Xmin, left.Xmax, right.Xmax }.Distinct().OrderBy(x => x).ToList();
    List<int> y = new[] { left.Ymin, right.Ymin, left.Ymax, right.Ymax }.Distinct().OrderBy(x => x).ToList();
    List<int> z = new[] { left.Zmin, right.Zmin, left.Zmax, right.Zmax }.Distinct().OrderBy(x => x).ToList();
    for (int iz = 0; iz < z.Count - 1; iz++)
    {
        for (int iy = 0; iy < y.Count - 1; iy++)
        {
            for (int ix = 0; ix < x.Count - 1; ix++)
            {
                Cube sub = new(x[ix], x[ix + 1], y[iy], y[iy + 1], z[iz], z[iz + 1]);
                if (left.Contains(sub) && right.Contains(sub))
                {
                    Debug.Assert(intersection == null);
                    intersection = sub;
                }
                else if (left.Contains(sub))
                {
                    leftRemainder.Add(sub);
                }
                else if (right.Contains(sub))
                {
                    rightRemainder.Add(sub);
                }
                // else - inside union bounding box, but not actually left or right
            }
        }
    }

    Debug.Assert(intersection != null);
    return true;
}

record Cube(int Xmin, int Xmax, int Ymin, int Ymax, int Zmin, int Zmax)
{
    public long Volume() => (Xmax - Xmin) * (Ymax - Ymin) * (Zmax - Zmin);

    public bool Contains(Cube other) =>
        other.Xmin >= Xmin && other.Xmax <= Xmax &&
        other.Ymin >= Ymin && other.Ymax <= Ymax &&
        other.Zmin >= Zmin && other.Zmax <= Zmax;

    public override string ToString() => $"x={Xmin}..{Xmax},y={Ymin}..{Ymax},z={Zmin}..{Zmax}";

    public IEnumerable<Point> Points()
    {
        for (int x = Xmin; x < Xmax; x++)
        {
            for (int y = Ymin; y < Ymax; y++)
            {
                for (int z = Zmin; z < Zmax; z++)
                {
                    yield return new Point(x, y, z);
                }
            }
        }
    }
}

record Point(int X, int Y, int Z) : IComparable<Point>
{
    public int CompareTo(Point? other)
    {
        if (other == null) return 1;
        if (other == this) return 0;
        if (X > other.X) return 1;
        if (X < other.X) return -1;
        if (Y > other.Y) return 1;
        if (Y < other.Y) return -1;
        if (Z > other.Z) return 1;
        if (Z < other.Z) return -1;
        Debug.Assert(false);
        return 0;
    }
}