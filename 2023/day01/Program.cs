var lines = File.ReadAllLines("input1.txt");
var sum = lines.Select(l =>
{
    var digits = l.Where(c => char.IsDigit(c)).ToArray();
    return int.Parse($"{digits[0]}{digits.Last()}");
}).Sum();
Console.WriteLine(sum);
