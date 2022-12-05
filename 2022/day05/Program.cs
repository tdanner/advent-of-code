using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");

int numStacks = (lines[0].Length + 1) / 4;
var stacks = new List<Stack<char>>();
for (int i = 0; i < numStacks; i++)
{
    stacks.Add(new Stack<char>());
}

int indexLine = 0;
while (lines[indexLine][1] != '1') indexLine++;
for (int i = 0; i < indexLine; i++)
{
    var line = lines[indexLine - i - 1];
    for (int j = 0; j < numStacks; j++)
    {
        char c = line[j * 4 + 1];
        if (c != ' ')
        {
            stacks[j].Push(c);
        }
    }
}

var moveParser = new Regex(@"^move (\d+) from (\d+) to (\d+)$");
foreach (var move in lines[(indexLine + 2)..])
{
    var m = moveParser.Match(move);
    int numToMove = int.Parse(m.Groups[1].ValueSpan);
    int source = int.Parse(m.Groups[2].ValueSpan);
    int target = int.Parse(m.Groups[3].ValueSpan);
    Console.WriteLine($"move {numToMove} from {source} to {target}");
    CrateMover9000(numToMove, source, target);
}

var part1 = new string(stacks.Select(s => s.Peek()).ToArray());
Console.WriteLine($"Part 1: {part1}");

void CrateMover9000(int numToMove, int source, int target)
{
    for (int i = 0; i < numToMove; i++)
    {
        stacks[target - 1].Push(stacks[source - 1].Pop());
    }
}
