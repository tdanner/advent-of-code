int[] crabs = File.ReadAllText("input.txt").Split(',').Select(int.Parse).ToArray();
Array.Sort(crabs);

int median = crabs[crabs.Length / 2];
int fuel = crabs.Select(crab => Math.Abs(crab - median)).Sum();
Console.WriteLine("Part 1: {0}", new { median, fuel });

long fuelCostForMove(int startPos, int endPos)
{
    int distance = Math.Abs(startPos - endPos);
    return distance * (distance + 1) / 2;
}
long totalFuelCostForPosition(int position)
{
    return crabs.Select(crab => fuelCostForMove(crab, position)).Sum();
}
var solution = Enumerable.Range(crabs[0], crabs[^1])
    .Select(pos => (fuel: totalFuelCostForPosition(pos), pos))
    .Min(x => x.fuel);
Console.WriteLine($"Part 2: {solution}");
