string input = File.ReadAllText("input1.txt");
string[] elfFoodItems = input.Split("\n\n");
var elfTotals = elfFoodItems.Select(elf => elf.Trim().Split("\n").Select(int.Parse).Sum());
int max = elfTotals.Max();
Console.WriteLine(max);
int top3Sum = elfTotals.OrderByDescending(n => n).Take(3).Sum();
Console.WriteLine(top3Sum);
