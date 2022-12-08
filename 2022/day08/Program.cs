using System.Text;

var lines = File.ReadAllLines("input.txt");
var visible = new bool[lines.Length, lines[0].Length];

for (int y = 0; y < lines.Length; y++)
{
    SetVisible(0, y, 1, 0);
    SetVisible(lines[0].Length - 1, y, -1, 0);
}
for (int x = 0; x < lines[0].Length; x++)
{
    SetVisible(x, 0, 0, 1);
    SetVisible(x, lines.Length - 1, 0, -1);
}

int part1 = 0;
var buffer = new StringBuilder();
for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[0].Length; x++)
    {
        if (visible[y, x])
        {
            buffer.Append('*');
            part1++;
        }
        else
        {
            buffer.Append(' ');
        }
    }
    buffer.AppendLine();
}

// Console.WriteLine(buffer.ToString());

Console.WriteLine($"Part 1: {part1}");


void SetVisible(int x, int y, int dx, int dy)
{
    int minHeight = -1;
    while (x >= 0 && x < lines[0].Length &&
           y >= 0 && y < lines.Length)
    {
        int height = lines[y][x] - '0';
        if (height > minHeight)
        {
            visible[y, x] = true;
        }
        minHeight = Math.Max(height, minHeight);
        x += dx;
        y += dy;
    }
}

var ymax = lines.Length;
var xmax = lines[0].Length;
var scenic = new int[ymax, xmax];
int maxScenic = 0;
for (int y = 0; y < ymax; y++)
{
    for (int x = 0; x < xmax; x++)
    {
        scenic[y, x] =
            GetVisibilityDistance(y, x, 0, 1) *
            GetVisibilityDistance(y, x, 0, -1) *
            GetVisibilityDistance(y, x, 1, 0) *
            GetVisibilityDistance(y, x, -1, 0);
        maxScenic = Math.Max(scenic[y, x], maxScenic);
    }
}

Console.WriteLine($"Part 2: {maxScenic}");

int h(int y, int x) => lines[y][x] - '0';

int GetVisibilityDistance(int y, int x, int dy, int dx)
{
    int h1 = h(y, x);
    int d = 0;
    while (true)
    {
        y += dy;
        x += dx;
        if (!(x >= 0 && x < xmax && y >= 0 && y < ymax))
        {
            break;
        }
        d++;
        if (h1 <= h(y, x))
        {
            break;
        }
    }
    return d;
}
