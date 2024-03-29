﻿using System.Text.RegularExpressions;

var text = File.ReadAllText("input.txt");
var parser = new Regex(@"
Monkey\s(?'num'\d+):
\s+Starting\ items:\ (?'items'[\d, ]+)
\s+Operation:\ new\ =\ old\ (?'operator'[+*])\ (?'operand'old|\d+)
\s+Test:\ divisible\ by\ (?'divisible'\d+)
\s+If\ true:\ throw\ to\ monkey\ (?'true'\d+)
\s+If\ false:\ throw\ to\ monkey\ (?'false'\d+)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

var monkeys = parser.Matches(text).Select(m => new Monkey
{
    items = m.Groups["items"].Value.Split(", ").Select(long.Parse).ToList(),
    operation = ParseOperation(m.Groups["operator"].Value, m.Groups["operand"].Value),
    divisibilityTest = int.Parse(m.Groups["divisible"].Value),
    monkeyIfTrue = int.Parse(m.Groups["true"].Value),
    monkeyIfFalse = int.Parse(m.Groups["false"].Value),
}).ToList();

int ring = monkeys.Select(m => m.divisibilityTest).Aggregate(1, (m, r) => r * m);

Func<long, long> ParseOperation(string oper, string operandStr)
{
    Func<long, long> operand = operandStr == "old"
        ? x => x
        : x => long.Parse(operandStr);
    return oper switch
    {
        "+" => (x => x + operand(x)),
        "*" => (x => x * operand(x)),
        _ => throw new Exception("bad operator")
    };
}

for (int round = 1; round <= 10000; round++)
{
    foreach (var m in monkeys)
    {
        foreach (long old in m.items)
        {
            m.inspections++;
            long item = m.operation(old);
            item %= ring;
            int target = (item % m.divisibilityTest == 0) ? m.monkeyIfTrue : m.monkeyIfFalse;
            monkeys[target].items.Add(item);
        }
        m.items.Clear();
    }

    if (round == 1 || round == 20 || (round % 1000 == 0))
    {
        Console.WriteLine($"\n == After round {round + 1} ==");
        for (int m = 0; m < monkeys.Count; m++)
        {
            Console.WriteLine($"Monkey {m} inspected items {monkeys[m].inspections} times.");
        }
    }
}

long part1 = monkeys.OrderByDescending(m => m.inspections).Take(2).Aggregate(1L, (a, m) => a * m.inspections);
Console.WriteLine($"Part 1: {part1}");

public class Monkey
{
    public List<long> items { get; set; } = new List<long>();
    public Func<long, long> operation = x => throw new NotImplementedException();
    public int divisibilityTest { get; set; }
    public int monkeyIfTrue { get; set; }
    public int monkeyIfFalse { get; set; }
    public int inspections { get; set; }
}
