using System.Collections;

var lines = File.ReadAllLines("input.txt");
int total = 0;
for (int g = 0; g < lines.Length; g += 3)
{
    var badges = Contents(lines[g]).And(Contents(lines[g + 1])).And(Contents(lines[g + 2]));
    for (char badge = 'A'; badge <= 'z'; badge++)
    {
        if (badges[badge])
        {
            total += GetPriority(badge);
            break;
        }
    }
}

Console.WriteLine(total);

BitArray Contents(string line)
{
    var result = new BitArray('z' + 1);
    for (int i = 0; i < line.Length; ++i)
    {
        result[line[i]] = true;
    }
    return result;
}

int GetPriority(char c) => c switch
{
    >= 'a' and <= 'z' => (c - 'a') + 1,
    >= 'A' and <= 'Z' => (c - 'A') + 27,
    _ => throw new Exception($"Unexpected char {c}")
};
