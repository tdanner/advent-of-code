string[] lines = File.ReadAllLines("input.txt");
string template = lines[0];
Dictionary<(char, char), char> rules = lines[2..].ToDictionary(line => (line[0], line[1]), line => line[6]);

Counter<(char, char)> pairCounts = new();
for (int pos = 0; pos < template.Length - 1; pos++)
{
    pairCounts[(template[pos], template[pos + 1])]++;
}

Counter<char> charCounts = new();
foreach (char c in template)
{
    charCounts[c]++;
}

for (int step = 1; step <= 40; step++)
{
    Counter<(char, char)> next = new();
    foreach (var pairCount in pairCounts)
    {
        char toInsert = rules[pairCount.Key];
        charCounts[toInsert] += pairCount.Value;
        next[(pairCount.Key.Item1, toInsert)] += pairCount.Value;
        next[(toInsert, pairCount.Key.Item2)] += pairCount.Value;
    }

    pairCounts = next;
}

long mostFrequent = charCounts.Values.Max();
long leastFrequent = charCounts.Values.Min();
Console.WriteLine(new { mostFrequent, leastFrequent, answer = mostFrequent - leastFrequent });
