using System.Text.RegularExpressions;

var matcher1 = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)");
var text = File.ReadAllText("input.txt");
var total = 0;
foreach (Match match in matcher1.Matches(text))
{
    int a = int.Parse(match.Groups[1].Value), b = int.Parse(match.Groups[2].Value);
    total += a * b;
}
Console.WriteLine($"Part 1: {total}");

var matcher2 = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)");
total = 0;
var enabled = true;
foreach (Match match in matcher2.Matches(text))
{
    if (match.Value == "do()")
    {
        enabled = true;
    }
    else if (match.Value == "don't()")
    {
        enabled = false;
    }
    else if (enabled)
    {
        int a = int.Parse(match.Groups[1].Value), b = int.Parse(match.Groups[2].Value);
        total += a * b;
    }
}
Console.WriteLine($"Part 2: {total}");
