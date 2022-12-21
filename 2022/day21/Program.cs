using System.Text.Json;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

var parser1 = new Regex(@"([a-z]+): (\d+)");
var parser2 = new Regex(@"([a-z]+): ([a-z]+) ([-+*/]) ([a-z]+)");
var monkeys = new Dictionary<string, Monkey>();
foreach (var line in lines)
{
    var m1 = parser1.Match(line);
    if (m1.Success)
    {
        monkeys[m1.Groups[1].Value] = new NumMonkey(long.Parse(m1.Groups[2].Value));
    }
    else
    {
        var m2 = parser2.Match(line);
        monkeys[m2.Groups[1].Value] = new ExprMonkey(m2.Groups[2].Value, m2.Groups[4].Value, m2.Groups[3].Value);
    }
}

Console.WriteLine($"Part 1: {monkeys["root"].Yell(monkeys)}");

abstract class Monkey
{
    public abstract long Yell(Dictionary<string, Monkey> monkeys);
}

class NumMonkey : Monkey
{
    public NumMonkey(long value)
    {
        Value = value;
    }
    public long Value { get; init; }
    public override long Yell(Dictionary<string, Monkey> monkeys) => Value;
}

class ExprMonkey : Monkey
{
    public string Left { get; init; }
    public string Right { get; init; }
    public string Oper { get; init; }

    public ExprMonkey(string left, string right, string oper)
    {
        Left = left;
        Right = right;
        Oper = oper;
    }

    public override long Yell(Dictionary<string, Monkey> monkeys)
    {
        long l = monkeys[Left].Yell(monkeys);
        long r = monkeys[Right].Yell(monkeys);
        return Oper switch
        {
            "+" => l + r,
            "-" => l - r,
            "*" => l * r,
            "/" => l / r,
            _ => throw new Exception($"unknown operator '{Oper}'"),
        };
    }
}
