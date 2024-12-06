using System.Text.Json;

var lines = File.ReadAllLines("input.txt").ToArray();
int blank = Array.IndexOf(lines, "");
var rules = new Dictionary<int, List<int>>();
var empty = new List<int>();
foreach (var line in lines[..blank])
{
    int first = int.Parse(line[0..2]);
    int second = int.Parse(line[3..5]);
    if (rules.TryGetValue(first, out var rule))
    {
        rule.Add(second);
    }
    else
    {
        rules.Add(first, [second]);
    }
}

// Console.WriteLine(JsonSerializer.Serialize(rules));

bool ruleExists(int first, int second)
{
    return rules.GetValueOrDefault(first, empty).Contains(second);
}

int ruleCompare(int a, int b)
{
    if (ruleExists(a, b))
    {
        return -1;
    }
    if (ruleExists(b, a))
    {
        return 1;
    }
    return 0;
}

int part1 = 0;
int part2 = 0;
foreach (var line in lines[(blank + 1)..])
{
    var pages = line.Split(",").Select(int.Parse).Index().ToDictionary(pair => pair.Item, pair => pair.Index);
    bool correct = true;
    foreach (var page in pages.Keys)
    {
        var firstPosition = pages[page];
        foreach (var mustPreceed in rules.GetValueOrDefault(page, empty))
        {
            if (pages.TryGetValue(mustPreceed, out var secondPosition))
            {
                if (firstPosition > secondPosition)
                {
                    // Console.WriteLine($"{line} violates {page}|{mustPreceed}");
                    correct = false;
                    break;
                }
            }
        }
        if (!correct)
        {
            break;
        }
    }
    if (correct)
    {
        var middle = int.Parse(line[(line.Length / 2 - 1)..(line.Length / 2 + 1)]);
        // Console.WriteLine($"{line} is correctly ordered (middle={middle})");
        part1 += middle;
    }
    else
    {
        var sorted = pages.Keys.ToList();
        sorted.Sort(ruleCompare);
        var middle = sorted[sorted.Count / 2];
        part2 += middle;
        // Console.WriteLine($"{line} becomes {string.Join(",", sorted)} (middle={middle})");
    }
}

Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");
