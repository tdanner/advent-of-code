var lines = File.ReadAllLines("input.txt");
int total = lines.Select(FindCommonChar).Select(GetPriority).Sum();
Console.WriteLine(total);

char FindCommonChar(string line)
{
    int mid = line.Length / 2;
    for (int i = 0; i < mid; i++)
    {
        for (int j = mid; j < line.Length; j++)
        {
            if (line[i] == line[j])
            {
                return line[i];
            }
        }
    }
    throw new Exception($"No common char found in {line}");
}

int GetPriority(char c) => c switch
{
    >= 'a' and <= 'z' => (c - 'a') + 1,
    >= 'A' and <= 'Z' => (c - 'A') + 27,
    _ => throw new Exception($"Unexpected char {c}")
};
