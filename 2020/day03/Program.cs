var lines = File.ReadAllLines("input.txt");
int height = lines.Length;
int width = lines[0].Length;
var map = (int x, int y) => lines[y][x % width];

int treeCount((int, int) slope)
{
    var (vx, vy) = slope;
    int x = 0, y = 0;
    int trees = 0;
    while (y < height)
    {
        if (map(x, y) == '#')
            trees++;
        x += vx;
        y += vy;

    }
    return trees;
}

(int, int)[] slopes = new[] {
    (1,1),
    (3,1),
    (5,1),
    (7,1),
    (1,2),
};

Console.WriteLine(slopes.Select(treeCount).Aggregate(1L, (a, b) => a * b));