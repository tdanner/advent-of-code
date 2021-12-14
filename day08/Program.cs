using TimDanner.Utils;

class Observation
{
    string[] patterns;
    public string[] outputs;
    Bimap<char, char> mapping = new();
    public Bimap<int, string> digits = new();

    public Observation(string[] patterns, string[] outputs)
    {
        this.patterns = patterns.Select(SortChars).ToArray();
        this.outputs = outputs.Select(SortChars).ToArray();

        InferMapping();
    }

    string SortChars(string s) => new string(s.OrderBy(c => c).ToArray());

    void InferMapping()
    {
        /*
            Rules:
            1. C and F are the only ones that occur in a 2-segment digit
            2. F occurs in all of the 6-segment digits but C does not
            -> C and F are identified
            3. A is the only segment that occurs in a 3-segment digit but not the 2-segment digit
            -> A is identified
            4. E and G occur in the 7-segment digit, but not the 2-, 3-, or 4- segment digits
            5. G occurs in all of the digits with >4 segments but not the 2-, 3-, or 4- segment digits
            -> E and G are identified
            -> 6, 0, and 9 are identified
            6. D is the only segment missing in 0
            -> D is identified
            7. By elimination, B is identified

6 ab defg
0 abc efg
9 abcd fg
        */

        digits[1] = patterns.Single(p => p.Length == 2);
        digits[4] = patterns.Single(p => p.Length == 4);
        digits[7] = patterns.Single(p => p.Length == 3);
        digits[8] = patterns.Single(p => p.Length == 7);

        // Rule 1 and 2
        if (patterns.Where(p => p.Length == 6).All(p => p.Contains(digits[1][0])))
        {
            mapping[digits[1][0]] = 'f';
            mapping[digits[1][1]] = 'c';
        }
        else
        {
            mapping[digits[1][0]] = 'c';
            mapping[digits[1][1]] = 'f';
        }

        // Rule 3
        mapping[digits[7].Except(digits[1]).Single()] = 'a';

        // Rule 4
        char[] e_g = digits[8]
            .Except(patterns.Where(p => p.Length <= 4).SelectMany(p => p)).ToArray();
        // Rule 5
        if (patterns.Where(p => p.Length > 4).All(p => p.Contains(e_g[0])))
        {
            mapping[e_g[0]] = 'g';
            mapping[e_g[1]] = 'e';
        }
        else
        {
            mapping[e_g[0]] = 'e';
            mapping[e_g[1]] = 'g';
        }

        digits[6] = patterns.Where(p => p.Length == 6)
            .Where(p => !p.Contains(mapping.GetByValue('c'))).Single();
        digits[9] = patterns.Where(p => p.Length == 6)
            .Where(p => !p.Contains(mapping.GetByValue('e'))).Single();
        digits[0] = patterns.Where(p => p.Length == 6).Except(digits.Values).Single();

        // Rule 6
        mapping[digits[8].Except(digits[0]).Single()] = 'd';

        // Rule 7
        mapping[digits[8].Except(mapping.Keys).Single()] = 'b';

        /*
        2 a cde g
        3 a cd fg
        5 ab d fg
        */
        digits[2] = patterns.Where(p => p.Length == 5 && p.Contains(mapping.GetByValue('e'))).Single();
        digits[5] = patterns.Where(p => p.Length == 5 && p.Contains(mapping.GetByValue('b'))).Single();
        digits[3] = patterns.Except(digits.Values).Single();
    }

    public int GetValue()
    {
        int value = 0;
        foreach (string output in outputs)
        {
            value *= 10;
            value += digits.GetByValue(output);
        }
        return value;
    }
}

class Program
{
    static void Main()
    {
        Observation[] observations = File.ReadAllLines("input.txt")
            .Select(line => line.Split('|', StringSplitOptions.TrimEntries))
            .Select(splits => new Observation(splits[0].Split(' '), splits[1].Split(' ')))
            .ToArray();

        // Part 1
        int[] uniquePatterns = { 2/*1*/, 3/*7*/, 4/*4*/, 7/*8*/ };
        int numUniquePatternsInOutput = observations
                .Select(obs => obs.outputs.Count(output => uniquePatterns.Contains(output.Length)))
                .Sum();
        Console.WriteLine(new { numUniquePatternsInOutput });

        // Part 2
        var total = observations.Select(obs => obs.GetValue()).Sum();
        Console.WriteLine(new { total });

        /*
        1   c  f  *
        4  bcd f  *
        7 a c  f  *
        2 a cde g
        3 a cd fg
        5 ab d fg
        6 ab defg
        0 abc efg
        9 abcd fg
        8 abcdefg *

          0:      1:      2:      3:      4:
         aaaa    ....    aaaa    aaaa    ....
        b    c  .    c  .    c  .    c  b    c
        b    c  .    c  .    c  .    c  b    c
         ....    ....    dddd    dddd    dddd
        e    f  .    f  e    .  .    f  .    f
        e    f  .    f  e    .  .    f  .    f
         gggg    ....    gggg    gggg    ....

          5:      6:      7:      8:      9:
         aaaa    aaaa    aaaa    aaaa    aaaa
        b    .  b    .  .    c  b    c  b    c
        b    .  b    .  .    c  b    c  b    c
         dddd    dddd    ....    dddd    dddd
        .    f  e    f  .    f  e    f  .    f
        .    f  e    f  .    f  e    f  .    f
         gggg    gggg    ....    gggg    gggg
                 */


    }
}
