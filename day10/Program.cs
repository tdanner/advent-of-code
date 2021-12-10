var lines = File.ReadAllLines("input.txt");
Dictionary<char, char> closers = new()
{
    ['('] = ')',
    ['['] = ']',
    ['{'] = '}',
    ['<'] = '>'
};

Dictionary<char, int> errorValues = new()
{
    [')'] = 3,
    [']'] = 57,
    ['}'] = 1197,
    ['>'] = 25137
};

var score = 0;
foreach (var line in lines)
{
    Stack<char> pending = new();
    foreach (var c in line)
        if (closers.TryGetValue(c, out var closer))
        {
            pending.Push(closer);
        }
        else
        {
            var expected = pending.Pop();
            if (c != expected)
            {
                score += errorValues[c];
                break;
            }
        }
}

Console.WriteLine(new { score });