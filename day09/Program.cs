var lines = File.ReadAllLines("input.txt");
int width = lines[0].Length + 2;
int length = lines.Length + 2;
int[,] heights = new int[width, length];

// Make a wall of height 10 around everything
for (int x = 0; x < width; x++)
{
    heights[x, 0] = 10;
    heights[x, length - 1] = 10;
}

for (int y = 0; y < length; y++)
{
    heights[0, y] = 10;
    heights[width - 1, y] = 10;
}

// Parse the input
for (int y = 1; y < length - 1; y++)
{
    for (int x = 1; x < width - 1; x++)
    {
        heights[x, y] = lines[y - 1][x - 1] - '0';
    }
}

// Part 1
int risk = 0;
for (int x = 1; x < width - 1; x++)
{
    for (int y = 1; y < length - 1; y++)
    {
        int height = heights[x, y];
        if (height < heights[x - 1, y] &&
            height < heights[x + 1, y] &&
            height < heights[x, y - 1] &&
            height < heights[x, y + 1])
        {
            risk += 1 + height;
        }
    }
}

Console.WriteLine(new { risk });

// Part 2
int[,] basinMap = new int[width, length];
void MarkBasin(int basin, int x, int y)
{
    if (basinMap[x, y] != 0 || heights[x, y] >= 9)
        return;

    basinMap[x, y] = basin;

    MarkBasin(basin, x - 1, y);
    MarkBasin(basin, x + 1, y);
    MarkBasin(basin, x, y - 1);
    MarkBasin(basin, x, y + 1);
}

for (int x = 1; x < width - 1; x++)
{
    for (int y = 1; y < length - 1; y++)
    {
        MarkBasin(y * length + x, x, y);
    }
}

int[] basinSizes = new int[length * width];
for (int x = 1; x < width - 1; x++)
{
    for (int y = 1; y < length - 1; y++)
    {
        basinSizes[basinMap[x, y]]++;
    }
}

basinSizes[0] = 0; // The zero basin is a count of all points not part of any basin

Array.Sort(basinSizes);
Console.WriteLine(new { basinProduct = basinSizes[^3..].Aggregate((a, b) => a * b) });
