using System.Text.RegularExpressions;

var numbers = new[]{
    "zeroxxxxxxxx", "one", "two", "three","four", "five", "six", "seven", "eight", "nine"
};

int ToNum(string v)
{
    if (v.Length == 1)
        return int.Parse(v);
    var i = Array.IndexOf(numbers, v);
    if (i < 0)
        throw new Exception($"didn't find {v}");
    return i;
}

var firstDigitRE = new Regex(string.Join("|", numbers) + "|[0-9]");
var lastDigitRE = new Regex(string.Join("|", numbers) + "|[0-9]", RegexOptions.RightToLeft);
var lines = File.ReadAllLines("input1.txt");
var sum = lines.Select(l =>
{
    var first = firstDigitRE.Match(l).Value;
    var last = lastDigitRE.Match(l).Value;
    return ToNum(first) * 10 + ToNum(last);
}).Sum();
Console.WriteLine(sum);
