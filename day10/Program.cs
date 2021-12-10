var lines = File.ReadAllLines("input.txt");
Dictionary<char, char> closers = new() { ['('] = ')', ['['] = ']', ['{'] = '}', ['<'] = '>' };

Dictionary<char, int> errorValues = new() { [')'] = 3, [']'] = 57, ['}'] = 1197, ['>'] = 25137 };

Dictionary<char, int> completionValues = new() { [')'] = 1, [']'] = 2, ['}'] = 3, ['>'] = 4 };

// Part 1

var errorScore = 0;
foreach (var line in lines)
{
    Stack<char> pending = new();
    foreach (var c in line)
    {
        if (closers.TryGetValue(c, out var closer))
        {
            pending.Push(closer);
        }
        else
        {
            var expected = pending.Pop();
            if (c != expected)
            {
                errorScore += errorValues[c];
                break;
            }
        }
    }
}

Console.WriteLine(new { score = errorScore });

// Part 2

List<long> completionScores = new();
foreach (var line in lines)
{
    Stack<char> pending = new();
    var corrupted = false;
    foreach (var c in line)
    {
        if (closers.TryGetValue(c, out var closer))
        {
            pending.Push(closer);
        }
        else
        {
            var expected = pending.Pop();
            if (c != expected)
            {
                errorScore += errorValues[c];
                corrupted = true;
                break;
            }
        }
    }

    if (!corrupted)
    {
        long completionScore = 0;
        while (pending.TryPop(out var c))
        {
            completionScore *= 5;
            completionScore += completionValues[c];
        }

        completionScores.Add(completionScore);
    }
}

completionScores.Sort();
Console.WriteLine(new { middleCompletionScore = completionScores[completionScores.Count / 2] });
