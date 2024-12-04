var lines = File.ReadAllLines("input.txt");
var pairs = lines.Select(l => l.Split("  ").Select(int.Parse));
var first = pairs.Select(p => p.ElementAt(0)).ToList();
var second = pairs.Select(p => p.ElementAt(1)).ToList();
first.Sort();
second.Sort();
var total = first.Zip(second).Select(p => Math.Abs(p.First - p.Second)).Sum();
Console.WriteLine("Part 1: " + total);

var occurrences = new Dictionary<int, int>();
second.ForEach(n => occurrences[n] = occurrences.GetValueOrDefault(n, 0) + 1);
var similarity = first.Select(n => occurrences.GetValueOrDefault(n, 0) * n).Sum();
Console.WriteLine("Part 2: " + similarity);
