using System.Diagnostics;
using System.Text.RegularExpressions;
using TimDanner.Utils;

List<(bool on, Cube cube)> steps = new();
foreach (string line in File.ReadAllLines("input.txt"))
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

SortedSet<int> xValues = new();
SortedSet<int> yValues = new();
SortedSet<int> zValues = new();
foreach (var step in steps)
{
    xValues.Add(step.cube.Xmin);
    xValues.Add(step.cube.Xmax);
    yValues.Add(step.cube.Ymin);
    yValues.Add(step.cube.Ymax);
    zValues.Add(step.cube.Zmin);
    zValues.Add(step.cube.Zmax);
}

xValues.Add(-50);
xValues.Add(51);
yValues.Add(-50);
yValues.Add(51);
zValues.Add(-50);
zValues.Add(51);

Console.WriteLine($"Distinct values: x={xValues.Count}, y={yValues.Count}, z={xValues.Count}");
Console.WriteLine($"stepSpace size: {xValues.Count * yValues.Count * xValues.Count:N0}");

Bimap<int, int> xGrid = new(), yGrid = new(), zGrid = new();
xValues.ForEach((x, i) => xGrid[x] = i);
yValues.ForEach((y, i) => yGrid[y] = i);
zValues.ForEach((z, i) => zGrid[z] = i);

bool[,,] stepSpace = new bool[zValues.Count, yValues.Count, xValues.Count];

foreach (var step in steps)
{
    for (int z = zGrid[step.cube.Zmin]; z < zGrid[step.cube.Zmax]; z++)
    {
        for (int y = yGrid[step.cube.Ymin]; y < yGrid[step.cube.Ymax]; y++)
        {
            for (int x = xGrid[step.cube.Xmin]; x < xGrid[step.cube.Xmax]; x++)
            {
                stepSpace[z, y, x] = step.on;
            }
        }
    }
}

long cubesOn = 0;
for (int z = zGrid[-50]; z < zGrid[51]; z++)
{
    for (int y = yGrid[-50]; y < yGrid[51]; y++)
    {
        for (int x = xGrid[-50]; x < xGrid[51]; x++)
        {
            if (stepSpace[z, y, x])
            {
                cubesOn += (zGrid.GetByValue(z + 1) - zGrid.GetByValue(z)) *
                           (yGrid.GetByValue(y + 1) - yGrid.GetByValue(y)) *
                           (xGrid.GetByValue(x + 1) - xGrid.GetByValue(x));
            }
        }
    }
}

Console.WriteLine(cubesOn);

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