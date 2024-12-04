bool IsSafe(List<int> levels)
{
    var diffs = new List<int>();
    for (int i = 1; i < levels.Count; i++)
    {
        diffs.Add(levels[i] - levels[i - 1]);
    }
    return diffs.All(d => d >= 1 && d <= 3) ||
        diffs.All(d => d >= -3 && d <= -1);
}

var lines = File.ReadAllLines("input.txt");
var part1 = lines.Select(l => l.Split(" ").Select(int.Parse).ToList()).Where(IsSafe).Count();
Console.WriteLine("Part 1: " + part1);
