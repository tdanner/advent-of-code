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

int score = 0;
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
        score += middle;
    }

}

Console.WriteLine($"Part 1: {score}");
