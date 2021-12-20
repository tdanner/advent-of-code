using System.Diagnostics;

string[] lines = File.ReadAllLines("example.txt");
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

Func<int, int, char> Enhance(Func<int, int, char> image, int numTimes)
{
    return (y, x) =>
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
                index |= image(y + sy, x + sx) == '#' ? 1 : 0;
            }
        }

        char c2 = algorithm[index];
        memo[(y, x, numTimes)] = c2;
        return c2;
    };
}

Console.WriteLine("Loaded image:");
Print(image, 0, height, 0, width);

image = Enhance(image, 50);

Console.WriteLine(new { pixelCount = CountPixelsOn(image, -50, height + 50, -50, width + 50) });

void Print(Func<int, int, char> image, int yMin, int yMax, int xMin, int xMax)
{
    for (int y = yMin; y < yMax; y++)
    {
        for (int x = xMin; x < xMax; x++)
        {
            Console.Write(image(y, x));
        }
        Console.WriteLine();
    }
}

int CountPixelsOn(Func<int, int, char> image, int yMin, int yMax, int xMin, int xMax)
{
    int count = 0;
    for (int y = yMin; y < yMax; y++)
    {
        for (int x = xMin; x < xMax; x++)
        {
            if (image(y, x) == '#') count++;
        }
    }

    return count;
}
