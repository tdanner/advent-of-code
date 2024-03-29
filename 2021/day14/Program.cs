﻿using TimDanner.Utils;

string[] lines = File.ReadAllLines("input.txt");
string template = lines[0];
Dictionary<(char, char), char> rules = lines[2..].ToDictionary(line => (line[0], line[1]), line => line[6]);

Counter<(char a, char b)> pairCounts = new();
for (int pos = 0; pos < template.Length - 1; pos++)
{
    pairCounts[(template[pos], template[pos + 1])]++;
}

Counter<char> charCounts = new();
template.ForEach(c => charCounts[c]++);

for (int step = 1; step <= 40; step++)
{
    Counter<(char, char)> next = new();
    foreach (var pairCount in pairCounts)
    {
        char toInsert = rules[pairCount.Key];
        charCounts[toInsert] += pairCount.Value;
        next[(pairCount.Key.a, toInsert)] += pairCount.Value;
        next[(toInsert, pairCount.Key.b)] += pairCount.Value;
    }

    pairCounts = next;
}

long mostFrequent = charCounts.Values.Max();
long leastFrequent = charCounts.Values.Min();
Console.WriteLine(new { mostFrequent, leastFrequent, answer = mostFrequent - leastFrequent });
