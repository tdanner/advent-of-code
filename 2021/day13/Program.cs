// Parse input
string[] lines = File.ReadAllLines("input.txt");
var points = lines.TakeWhile(line => line != "")
                  .Select(line => (int.Parse(line[..line.IndexOf(',')]),
                                   int.Parse(line[(line.IndexOf(',') + 1)..])))
                  .ToArray();
var folds = lines.SkipWhile(line => !line.StartsWith("fold"))
                 .Select(line => (line[line.IndexOf('=') - 1],
                                  int.Parse(line[(line.IndexOf('=') + 1)..])))
                 .ToArray();

int width = points.Select(p => p.Item1).Max() + 1;
int height = points.Select(p => p.Item2).Max() + 1;

bool[,] grid = new bool[width, height];
foreach (var point in points)
{
    grid[point.Item1, point.Item2] = true;
}

bool first = true;
foreach (var fold in folds)
{
    if (fold.Item1 == 'x')
    {
        grid = FoldLeft(grid, fold.Item2);
    }
    else
    {
        grid = FoldUp(grid, fold.Item2);
    }

    if (first)
    {
        first = false;
        int dotsAfterFirstFold = 0;
        foreach (var dot in grid)
        {
            if (dot) dotsAfterFirstFold++;
        }

        Console.WriteLine(new { dotsAfterFirstFold });
    }
}

PrintGrid(grid);

void PrintGrid(bool[,] grid)
{
    Console.WriteLine();
    for (int y = 0; y < grid.GetLength(1); ++y)
    {
        Console.WriteLine(string.Join(null, Enumerable.Range(0, grid.GetLength(0))
                                                      .Select(x => grid[x, y] ? '#' : '.')));
    }
}

bool[,] FoldLeft(bool[,] grid, int foldX)
{
    int leftWidth = foldX;
    int rightWidth = grid.GetLength(0) - foldX - 1;
    int foldedWidth = Math.Max(leftWidth, rightWidth);
    bool[,] folded = new bool[foldedWidth, grid.GetLength(1)];
    for (int y = 0; y < folded.GetLength(1); ++y)
    {
        for (int x = 0; x < folded.GetLength(0); ++x)
        {
            folded[x, y] = grid[x, y];
            folded[x, y] |= grid[foldX + (foldX - x), y];
        }
    }

    return folded;
}

bool[,] FoldUp(bool[,] grid, int foldY)
{
    int topHeight = foldY;
    int bottomHeight = grid.GetLength(1) - foldY - 1;
    int foldedHeight = Math.Max(topHeight, bottomHeight);
    bool[,] folded = new bool[grid.GetLength(0), foldedHeight];
    for (int y = 0; y < foldedHeight; ++y)
    {
        for (int x = 0; x < folded.GetLength(0); ++x)
        {
            folded[x, y] = grid[x, y];
            folded[x, y] |= grid[x, foldY + (foldY - y)];
        }
    }

    return folded;
}

