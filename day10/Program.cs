string[] lines = File.ReadAllLines("input.txt");
Dictionary<char, char> closers = new() { ['('] = ')', ['['] = ']', ['{'] = '}', ['<'] = '>' };

Dictionary<char, int> errorValues = new() { [')'] = 3, [']'] = 57, ['}'] = 1197, ['>'] = 25137 };

Dictionary<char, int> completionValues = new() { [')'] = 1, [']'] = 2, ['}'] = 3, ['>'] = 4 };

// Part 1

int errorScore = 0;
foreach (string line in lines)
{
    Stack<char> pending = new();
    foreach (char c in line)
    {
        if (closers.TryGetValue(c, out char closer))
        {
            pending.Push(closer);
        }
        else
        {
            char expected = pending.Pop();
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
foreach (string line in lines)
{
    Stack<char> pending = new();
    bool corrupted = false;
    foreach (char c in line)
    {
        if (closers.TryGetValue(c, out char closer))
        {
            pending.Push(closer);
        }
        else
        {
            char expected = pending.Pop();
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
        while (pending.TryPop(out char c))
        {
            completionScore *= 5;
            completionScore += completionValues[c];
        }

        completionScores.Add(completionScore);
    }
}

completionScores.Sort();
Console.WriteLine(new { middleCompletionScore = completionScores[completionScores.Count / 2] });
