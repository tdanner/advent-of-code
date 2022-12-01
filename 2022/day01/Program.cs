string input = File.ReadAllText("input1.txt");
string[] elves = input.Split("\n\n");
int max = elves.Select(elf => elf.Trim().Split("\n").Select(int.Parse).Sum()).Max();
Console.WriteLine(max);
