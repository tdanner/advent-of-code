using System.Diagnostics;

// Parse input
const string start = "start";
const string end = "end";
string[] lines = File.ReadAllLines("input.txt");
var edges = lines.Select(line => (line[..line.IndexOf('-')], line[(line.IndexOf('-') + 1)..]));
List<(string, string)> map = edges.Union(edges.Where(edge => edge.Item1 != start && edge.Item2 != end)
                                              .Select(edge => (edge.Item2, edge.Item1)))
                                  .ToList();

bool IsBig(string cave) => char.IsUpper(cave[0]);
bool IsSmall(string cave) => !char.IsUpper(cave[0]);

Debug.Assert(!map.Where(edge => IsBig(edge.Item1) && IsBig(edge.Item2)).Any());

// Part 2
bool NoSmallCaveVisitedTwice(cons path)
{
    return !path.Items().Where(IsSmall).GroupBy(x => x).Any(g => g.Count() > 1);
}

IEnumerable<cons> FindPaths(cons prefix)
{
    foreach (var (here, nextCave) in map.Where(edge => edge.Item1 == prefix.car))
    {
        if (nextCave == start)
        {
            continue; // can't visit start twice
        }
        else if (nextCave == end)
        {
            yield return new cons(nextCave, prefix);
        }
        else if (IsBig(nextCave) || !prefix.Contains(nextCave) || NoSmallCaveVisitedTwice(prefix))
        {
            foreach (var path in FindPaths(new cons(nextCave, prefix)))
            {
                yield return path;
            }
        }
    }
}

var paths = FindPaths(new cons(start, null)).ToList();
// paths.ForEach(Console.WriteLine);
Console.WriteLine(new { paths.Count });

void PrintPath(cons path)
{
    if (path.cdr != null)
    {
        PrintPath(path.cdr);
        Console.Write(",");
    }
    Console.Write(path.car);
}

record cons(string car, cons? cdr)
{
    public bool Contains(string value)
    {
        return car == value || (cdr?.Contains(value) ?? false);
    }

    // Reversed!
    public override string ToString()
    {
        return cdr == null ? car : $"{cdr},{car}";
    }

    public IEnumerable<string> Items()
    {
        cons? here = this;
        while (here != null)
        {
            yield return here.car;
            here = here.cdr;
        }
    }
}
