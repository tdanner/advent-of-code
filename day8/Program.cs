record Observation(string[] patterns, string[] outputs)
{
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
        int[] uniquePatterns = { 2, 3, 4, 7 };
        int numUniquePatternsInOutput = observations
                .Select(obs => obs.outputs.Count(output => uniquePatterns.Contains(output.Length)))
                .Sum();
        Console.WriteLine(new { numUniquePatternsInOutput });
    }
}
