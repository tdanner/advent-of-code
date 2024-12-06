using System.Text.RegularExpressions;

var matcher = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");
var text = File.ReadAllText("input.txt");
var total = 0;
foreach (Match match in matcher.Matches(text))
{
    int a = int.Parse(match.Groups[1].Value), b = int.Parse(match.Groups[2].Value);
    total += a * b;
}
Console.WriteLine($"Part 1: {total}");
