using static System.Math;

var lines = File.ReadAllLines("input.txt");
var visited = new HashSet<pt>();

var start = new pt(0, 4);
var head = start;
var tail = head;

const int w = 6, h = 5;

Console.WriteLine($"== Initial State ==");
dump();

foreach (var line in lines)
{
    char dir = line[0];
    int dist = int.Parse(line[2..]);
    Console.WriteLine($"== {dir} {dist} ==");
    for (int _ = 0; _ < dist; _++)
    {
        head = head.move(dir);
        tail = tail.follow(head);
        visited.Add(tail);
        dump();
    }
}

Console.WriteLine($"Part 1: {visited.Count}");

void dump()
{
    var buffer = new System.Text.StringBuilder();
    for (int y = 0; y < h; y++)
    {
        for (int x = 0; x < w; x++)
        {
            var here = new pt(x, y);
            char c;
            if (here == head)
                c = 'H';
            else if (here == tail)
                c = 'T';
            else if (here == start)
                c = 's';
            else
                c = '.';
            buffer.Append(c);

        }
        buffer.AppendLine();
    }
    Console.WriteLine(buffer.ToString());
}


record class pt(int x, int y)
{
    public pt move(char dir) => dir switch
    {
        'R' => new pt(x + 1, y),
        'L' => new pt(x - 1, y),
        'U' => new pt(x, y - 1),
        'D' => new pt(x, y + 1),
        _ => throw new Exception($"bad move '{dir}'")
    };

    public pt follow(pt other)
    {
        int dx = other.x - x,
            dy = other.y - y;

        if (Abs(dx) <= 1 && Abs(dy) <= 1)
        {
            return this;
        }
        else
        {
            return new pt(x + Math.Sign(dx), y + Math.Sign(dy));
        }
    }
}
