var lines = File.ReadAllLines("input.txt");
char get(int x, int y)
{
    if (y < 0 || y >= lines.Length || x < 0 || x >= lines[0].Length)
    {
        return ' ';
    }
    return lines[y][x];
}

var directions = new (int, int)[] {
    (1, 0),
    (0, 1),
    (1, 1),
    (-1, 0),
    (0, -1),
    (-1, -1),
    (1, -1),
    (-1, 1),
};

const string word = "XMAS";
int found = 0;
for (int y = 0; y < lines.Length; y++)
{
    for (int x = 0; x < lines[0].Length; x++)
    {
        foreach (var dir in directions)
        {
            bool match = true;
            for (int c = 0; c < word.Length; c++)
            {
                if (get(x + dir.Item1 * c, y + dir.Item2 * c) != word[c])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                found++;
            }
        }
    }
}

Console.WriteLine($"Part 1: {found}");
