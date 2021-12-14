string[] lines = File.ReadAllLines("input.txt");
int bitCount = lines[0].Length;
int[] ones = new int[bitCount];
foreach (string line in lines)
{
    for (int i = 0; i < bitCount; ++i)
    {
        if (line[i] == '1')
        {
            ones[i]++;
        }
    }
}

uint gamma = 0;
for (int i = 0; i < bitCount; ++i)
{
    gamma <<= 1;
    if (ones[i] > lines.Length / 2)
    {
        gamma++;
    }
}

uint mask = (((uint)1) << bitCount) - 1;
uint epsilon = ~gamma & mask;
uint powerConsumption = gamma * epsilon;
Console.WriteLine(new { gamma, epsilon, powerConsumption });

//////////////////////////////////////////////////////////////

List<string> nums = lines.ToList();

int CountOnesInPosition(int pos)
{
    int ones = 0;
    foreach (string num in nums)
    {
        if (num[pos] == '1')
        {
            ones++;
        }
    }

    return ones;
}

for (int i = 0; i < bitCount; ++i)
{
    int ones2 = CountOnesInPosition(i);
    char mostCommon = ones2 >= nums.Count / 2.0 ? '1' : '0';
    nums = nums.Where(n => n[i] == mostCommon).ToList();
}

int oxygenGeneratorRating = Convert.ToInt32(nums.Single(), 2);

nums = lines.ToList();

for (int i = 0; i < bitCount && nums.Count > 1; ++i)
{
    int ones2 = CountOnesInPosition(i);
    char leastCommon = ones2 >= nums.Count / 2.0 ? '0' : '1';
    nums = nums.Where(n => n[i] == leastCommon).ToList();
}

int co2ScrubberRating = Convert.ToInt32(nums.Single(), 2);

int lifeSupportRating = oxygenGeneratorRating * co2ScrubberRating;

Console.WriteLine(new { oxygenGeneratorRating, co2ScrubberRating, lifeSupportRating });
