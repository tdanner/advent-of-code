using System.Diagnostics;

string[] lines = File.ReadAllLines("input.txt");
string algorithm = lines[0];
Debug.Assert(algorithm.Length == 512);
int height = lines.Length - 2;
int width = lines[2].Length;

Dictionary<(int, int, int), char> memo = new();

Func<int, int, char> image = (y, x) =>
{
    if (memo.TryGetValue((y, x, 0), out char c)) return c;

    if (y < 0 || y >= lines.Length - 2)
        return '.';
    if (x < 0 || x >= lines[2].Length)
        return '.';

    char c2 = lines[y + 2][x];
    memo[(y, x, 0)] = c2;
    return c2;
};

char Enhanced(Func<int, int, char> image, int y, int x, int numTimes)
{
    if (memo.TryGetValue((y, x, numTimes), out char c)) return c;
    if (numTimes == 0)
    {
        return image(y, x);
    }

    int index = 0;
    for (int sy = -1; sy <= 1; sy++)
    {
        for (int sx = -1; sx <= 1; sx++)
        {
            index <<= 1;
            index |= Enhanced(image, y + sy, x + sx, numTimes - 1) == '#' ? 1 : 0;
        }
    }

    char c2 = algorithm[index];
    memo[(y, x, numTimes)] = c2;
    return c2;
}

int count = 0;
for (int y = -50; y < height + 50; y++)
{
    for (int x = -50; x < width + 50; x++)
    {
        if (Enhanced(image, y, x, 50) == '#') count++;
    }
}

Console.WriteLine(new { count });
