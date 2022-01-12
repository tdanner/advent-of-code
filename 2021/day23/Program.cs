using System.Text;

string[] lines = File.ReadAllLines("example.txt");
char[,] map = ParseMap(lines);
World world = new World { map = map };
foreach (var move in world.PossibleMoves())
{
    Console.WriteLine(move.Item1);
    Console.WriteLine(move.Item2);
}

char[,] ParseMap(string[] lines)
{
    var map = new char[5, 13];
    for (int y = 0; y < 5; y++)
    {
        for (int x = 0; x < 13; x++)
        {
            map[y, x] = lines[y][x];
        }
    }
    return map;
}

record Amphipod(char type, Point loc)
{
    public int MoveCost => type switch
    {
        'A' => 1,
        'B' => 10,
        'C' => 100,
        'D' => 1000,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown type"),
    };
}

struct World
{
    public char[,] map = new char[13, 5];
    Amphipod GetAmphipod(int i)
    {
        char type = ((char)('A' + i / 2));
        bool first = i % 2 == 0;
        for (int y = 1; y < 4; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                if (map[y, x] == type)
                {
                    if (first)
                    {
                        return new Amphipod(type, new(x, y));
                    }
                    else
                    {
                        first = true;
                    }
                }
            }
        }

        throw new Exception("Oh no! We lost one!");
    }
    public override string ToString()
    {
        StringBuilder buffer = new();
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 13; x++)
            {
                buffer.Append(map[y, x]);
            }
            buffer.AppendLine();
        }
        return buffer.ToString();
    }

    static (int x, int y)[] moves = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };

    public IEnumerable<(int, World)> PossibleMoves()
    {
        for (int i = 0; i < 8; i++)
        {
            var pod = GetAmphipod(i);
            foreach (var move in moves)
            {
                if (map[pod.loc.Y + move.y, pod.loc.X + move.x] == '.')
                {
                    World after = Duplicate();
                    after.map[pod.loc.Y, pod.loc.X] = '.';
                    after.map[pod.loc.Y + move.y, pod.loc.X + move.x] = pod.type;
                    yield return (pod.MoveCost, after);
                }
            }
        }
    }

    World Duplicate()
    {
        char[,] map2 = new char[5, 13];
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 13; x++)
            {
                map2[y, x] = map[y, x];
            }
        }
        return new World { map = map2 };
    }
}

record Point(int X, int Y);

/*
#############
#...........#
###A#B#C#D###
  #A#B#C#D#
  #########
*/
