string[] lines = File.ReadAllLines("example.txt");
string template = lines[0];
Dictionary<(char, char), char> rules = lines[2..].ToDictionary(line => (line[0], line[1]), line => line[6]);

Counter<(char, char)> pairCounts = new();
for (int pos = 0; pos < template.Length - 1; pos++)
{
    pairCounts[(template[pos], template[pos + 1])] = 1;
}

for (int step = 1; step <= 40; step++)
{
    Counter<(char, char)> next = new();
    foreach (var pairCount in pairCounts)
    {
        char toInsert = rules[pairCount.Key];
        next[(pairCount.Key.Item1, toInsert)] += pairCount.Value;
        next[(toInsert, pairCount.Key.Item2)] += pairCount.Value;
    }

    pairCounts = next;
}

Counter<char> charCounts = new();
foreach (var pair in pairCounts)
{
    charCounts[pair.Key.Item1] += pair.Value;
    charCounts[pair.Key.Item2] += pair.Value;
}
// Now all characters have been double-counted except the first and last ones of the template
charCounts[template[0]]++;
charCounts[template[^1]]++;
foreach (char c in charCounts.Keys)
{
    charCounts[c] /= 2;
}

long mostFrequent = charCounts.Values.Max();
long leastFrequent = charCounts.Values.Min();
Console.WriteLine(new { mostFrequent, leastFrequent, answer = mostFrequent - leastFrequent });
