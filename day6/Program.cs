List<int> lanternfishAges = File.ReadAllText("input.txt").Split(',')
    .Select(int.Parse).ToList();

for (int day = 0; day < 80; day++)
{
    List<int> newFish = new();
    foreach (int age in lanternfishAges)
    {
        if (age == 0)
        {
            newFish.Add(6);
            newFish.Add(8);
        }
        else
        {
            newFish.Add(age - 1);
        }
    }
    lanternfishAges = newFish;
}

Console.WriteLine(new { lanternfishCount = lanternfishAges.Count });
