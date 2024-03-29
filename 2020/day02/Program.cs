﻿using System.Text.RegularExpressions;

var passwords = File.ReadAllLines("input.txt")
            .Select(l => Regex.Match(l, @"^(\d+)-(\d+) ([a-z]): ([a-z]+)$"))
            .Select(m => new Password(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), m.Groups[3].Value[0], m.Groups[4].Value))
            .ToArray();

Console.WriteLine(passwords.Count(p => p.IsValidPart1()));
Console.WriteLine(passwords.Count(p => p.IsValidPart2()));

record Password(int Min, int Max, char Required, string Actual)
{
    public bool IsValidPart1()
    {
        int c = Actual.Count(c => c == Required);
        return c >= Min && c <= Max;
    }

    public bool IsValidPart2()
    {
        return (Actual[Min - 1] == Required) ^ (Actual[Max - 1] == Required);
    }
}