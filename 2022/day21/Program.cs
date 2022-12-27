using System.Text.Json;
using System.Text.RegularExpressions;

internal class Program
{
    public static Dictionary<string, Monkey> monkeys = new();

    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");

        var parser1 = new Regex(@"([a-z]+): (\d+)");
        var parser2 = new Regex(@"([a-z]+): ([a-z]+) ([-+*/]) ([a-z]+)");
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

        Console.WriteLine($"Part 1: {monkeys["root"].Yell()}");

        var humn = (NumMonkey)monkeys["humn"];
        var root = (ExprMonkey)monkeys["root"];

        var testNum = (long num) =>
        {
            humn.Value = num;
            long l = monkeys[root.Left].Yell();
            long r = monkeys[root.Right].Yell();
            return l - r;
        };

        long lower = 1;
        long upper = 4398046511104;

        for (int j = 0; j < 100; j++)
        {
            long i = (upper + lower) / 2;
            long diff = testNum(i);
            Console.WriteLine($"{i} diff: {diff}");
            if (diff == 0)
            {
                Console.WriteLine($"Part 2: {i}");
                upper--;
            }
            else if (diff > 0)
            {
                lower = i;
            }
            else
            {
                upper = i;
            }
        }
    }
}

abstract class Monkey
{
    public abstract long Yell();
}

class NumMonkey : Monkey
{
    public NumMonkey(long value)
    {
        Value = value;
    }
    public long Value { get; set; }
    public override long Yell() => Value;
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

    public override long Yell()
    {
        long l = Program.monkeys[Left].Yell();
        long r = Program.monkeys[Right].Yell();

        long v = Oper switch
        {
            "+" => l + r,
            "-" => l - r,
            "*" => l * r,
            "/" => l / r,
            _ => throw new Exception($"unknown operator '{Oper}'"),
        };

        return v;
    }
}
