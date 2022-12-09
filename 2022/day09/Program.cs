using static System.Math;

var lines = File.ReadAllLines("input.txt");
var visited = new HashSet<pt>();

var start = new pt(11, 15);
var knots = Enumerable.Repeat(start, 10).ToArray();

const int w = 26, h = 20;

//Console.WriteLine($"== Initial State ==");
//dump();

foreach (var line in lines)
{
    char dir = line[0];
    int dist = int.Parse(line[2..]);
    //Console.WriteLine($"== {dir} {dist} ==");
    for (int _ = 0; _ < dist; _++)
    {
        knots[0] = knots[0].move(dir);
        for (int i = 1; i < knots.Length; i++)
        {
            knots[i] = knots[i].follow(knots[i - 1]);
        }

        visited.Add(knots[^1]);
        //dump();
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
            char c = '.';
            if (here == start)
            {
                c = 's';
            }
            for (int i = knots.Length - 1; i >= 1; i--)
            {
                if (here == knots[i])
                {
                    c = (char)('0' + i);
                }
            }
            if (here == knots[0])
            {
                c = 'H';
            }
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
