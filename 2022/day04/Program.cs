var lines = File.ReadAllLines("input.txt");
var parser = new System.Text.RegularExpressions.Regex(@"^(\d+)-(\d+),(\d+)-(\d+)$");
var pairs = lines.Select(line => parser.Match(line)).Select(m => Tuple.Create(
    Tuple.Create(int.Parse(m.Groups[1].ValueSpan), int.Parse(m.Groups[2].ValueSpan)),
    Tuple.Create(int.Parse(m.Groups[3].ValueSpan), int.Parse(m.Groups[4].ValueSpan))))
    .ToArray();

int part1 = pairs.Where(p => AContainsB(p.Item1, p.Item2) || AContainsB(p.Item2, p.Item1)).Count();
Console.WriteLine($"Part 1: {part1}");

int part2 = pairs.Where(p => Overlaps(p.Item1, p.Item2)).Count();
Console.WriteLine($"Part 2: {part2}");

bool AContainsB(Tuple<int, int> a, Tuple<int, int> b)
{
    return a.Item1 >= b.Item1 && a.Item2 <= b.Item2;
}

bool Overlaps(Tuple<int, int> a, Tuple<int, int> b)
{
    int start = Math.Max(a.Item1, b.Item1);
    int end = Math.Min(a.Item2, b.Item2);
    return start <= end;
}
