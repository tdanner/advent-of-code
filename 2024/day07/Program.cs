var lines = File.ReadAllLines("input.txt");
long part1 = 0;
foreach (var line in lines)
{
    var (target, nums) = line.Split(": ", 2) switch
    {
    [var first, var second] => (long.Parse(first), second.Split(" ").Select(long.Parse).ToArray()),
        _ => throw new Exception("blah")
    };
    if (CanCompute(target, nums))
    {
        part1 += target;
    }
}

Console.WriteLine($"Part 1: {part1}");

static bool CanCompute(long target, Span<long> nums)
{
    return nums.Length == 1 && target == nums[0] ||
     nums.Length > 1 && (CanCompute(target - nums[^1], nums[..^1]) ||
        (target % nums[^1] == 0 && CanCompute(target / nums[^1], nums[..^1])));
}
