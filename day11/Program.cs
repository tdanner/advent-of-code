// Parse input
string[] lines = File.ReadAllLines("input.txt");
int width = lines[0].Length;
int height = lines.Length;
int[,] initialGrid = new int[width, height];
for (int y = 0; y < height; ++y)
{
    for (int x = 0; x < width; ++x)
    {
        initialGrid[x, y] = lines[y][x] - '0';
    }
}

// Part 1
int[,] grid = (int[,])initialGrid.Clone();
int flashes = 0;

IEnumerable<(int x, int y)> AllNeighbors(int x, int y)
{
    for (int yOff = -1; yOff <= 1; ++yOff)
    {
        for (int xOff = -1; xOff <= 1; ++xOff)
        {
            yield return (x + xOff, y + yOff);
        }
    }
}

IEnumerable<(int, int)> Neighbors(int xBase, int yBase)
{
    return AllNeighbors(xBase, yBase).Where(
        p => p.x >= 0 && p.x < width &&
             p.y >= 0 && p.y < height &&
             !(p.x == xBase && p.y == yBase));
}

void MaybeFlash(int x, int y)
{
    if (grid[x, y] == 10)
    {
        flashes++;
        // just bump it up to 11 so we know its flash has already been counted
        grid[x, y] = 11;
        foreach (var (x2, y2) in Neighbors(x, y))
        {
            if (grid[x2, y2] < 10)
            {
                grid[x2, y2]++;
                MaybeFlash(x2, y2);
            }
        }
    }
}

for (int step = 1; step <= 100; step++)
{
    // Increase all energy levels by 1
    for (int y = 0; y < height; ++y)
    {
        for (int x = 0; x < width; ++x)
        {
            grid[x, y]++;
        }
    }

    // Propagate flashes
    for (int y = 0; y < height; ++y)
    {
        for (int x = 0; x < width; ++x)
        {
            MaybeFlash(x, y);
        }
    }

    // Reset levels > 9 to 0
    for (int y = 0; y < height; ++y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] > 9)
            {
                grid[x, y] = 0;
            }
        }
    }

    // Console.WriteLine($"After step {step}:");
    // for (int y = 0; y < height; ++y)
    // {
    //     Console.WriteLine(string.Join(null, Enumerable.Range(0, width).Select(x => grid[x, y])));
    // }
    // Console.WriteLine();
}

Console.WriteLine(new { flashes });
