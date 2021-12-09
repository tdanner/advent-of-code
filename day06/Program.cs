List<int> lanternfishAges = File.ReadAllText("input.txt").Split(',')
    .Select(int.Parse).ToList();

long[] fishCountByAge = new long[9];
foreach (int age in lanternfishAges)
{
    fishCountByAge[age]++;
}

for (int day = 0; day < 256; day++)
{
    long[] newFish = new long[9];
    for (int age = 0; age < fishCountByAge.Length - 1; age++)
    {
        newFish[age] = fishCountByAge[age + 1];
    }
    newFish[8] = fishCountByAge[0];
    newFish[6] += fishCountByAge[0];
    fishCountByAge = newFish;
}

Console.WriteLine(new { lanternfishCount = fishCountByAge.Sum() });
