var lines = File.ReadAllLines("input.txt");
List<int> xValues = new();
xValues.Add(-999); // just a value to take up cycle zero, which doesn't exist
int xReg = 1;
foreach (var line in lines)
{
    xValues.Add(xReg);
    if (line == "noop")
    {
        // noop
    }
    else if (line.StartsWith("addx "))
    {
        int dx = int.Parse(line[5..]);
        xValues.Add(xReg);
        xReg += dx;
    }
}
xValues.Add(xReg);

for (int i = 20; i < xValues.Count; i += 40)
{
    Console.WriteLine($"{i,3}: {xValues[i],5} signal: {i * xValues[i],6}");
}

int part1 = Enumerable.Range(0, 6).Select(i => i * 40 + 20).Select(c => c * xValues[c]).Sum();
Console.WriteLine($"Part 1: {part1}");

int width = 40;
var buffer = new System.Text.StringBuilder();
for (int cycle = 1; cycle < xValues.Count; cycle++)
{
    int x = ((cycle - 1) % width);
    bool on = xValues[cycle] >= x - 1 && xValues[cycle] <= x + 1;
    buffer.Append(on ? '#' : '.');
    if (cycle % 40 == 0)
    {
        buffer.AppendLine();
    }
}
Console.WriteLine(buffer);
