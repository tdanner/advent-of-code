using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var sw = Stopwatch.StartNew();
        var lines = File.ReadAllLines("input.txt");
        long part1 = 0;
        long part2 = 0;
        foreach (var line in lines)
        {
            var (target, nums) = line.Split(": ", 2) switch
            {
            [var first, var second] => (long.Parse(first), second.Split(" ").Select(long.Parse).ToArray()),
                _ => throw new Exception("blah")
            };
            if (CanCompute1(target, nums))
            {
                part1 += target;
            }
            if (CanCompute2(target, nums))
            {
                part2 += target;
            }
        }

        Console.WriteLine($"Part 1: {part1}");
        Console.WriteLine($"Part 2: {part2}");
        Console.WriteLine($"Runtime: {sw.Elapsed.TotalMilliseconds}ms");
    }

    static bool CanCompute1(long target, Span<long> nums)
    {
        return nums.Length == 1 && target == nums[0] ||
             nums.Length > 1 &&
                (CanCompute1(target - nums[^1], nums[..^1]) ||
                target % nums[^1] == 0 && CanCompute1(target / nums[^1], nums[..^1]));
    }

    static bool CanCompute2(long target, Span<long> nums)
    {
        return nums.Length == 1 && target == nums[0] ||
             nums.Length > 1 &&
                (CanCompute2(target - nums[^1], nums[..^1]) ||
                 (target % nums[^1] == 0 && CanCompute2(target / nums[^1], nums[..^1])) ||
                 (target % Magnitude(nums[^1]) == nums[^1] && CanCompute2(target / Magnitude(nums[^1]), nums[..^1]))
                );
    }

    static long Magnitude(long n)
    {
        long m = 10;
        while (n >= m)
        {
            m *= 10;
        }
        return m;
    }
}
