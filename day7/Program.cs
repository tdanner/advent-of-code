int[] crabs = File.ReadAllText("input.txt").Split(',').Select(int.Parse).ToArray();
Array.Sort(crabs);
int median = crabs[crabs.Length / 2];
int fuel = crabs.Select(crab => Math.Abs(crab - median)).Sum();
Console.WriteLine(new { median, fuel });
