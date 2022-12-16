using System.Text.Json;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines("sample.txt");
var parser = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnels? leads? to valves? ([A-Z, ]+)");
var flowRates = new Dictionary<string, int>();
var tunnels = new Dictionary<string, string[]>();
foreach (var line in lines)
{
    var m = parser.Match(line);
    var id = m.Groups[1].Value;
    flowRates[id] = int.Parse(m.Groups[2].Value);
    tunnels[id] = m.Groups[3].Value.Split(", ");
}
Console.WriteLine(JsonSerializer.Serialize(flowRates));
Console.WriteLine(JsonSerializer.Serialize(tunnels));

// First cut: greedy algorithm. Go to the valve that will release the most total pressure,
// taking into account the time remaining and the time required to get there. And opening 
// valves along the way, I guess?
var openValves = new List<string>();
int time = 1;
string loc = "AA";
int pressureReleased = 0;
while (time <= 30)
{
    // calculated the expected value of moving to each valve
    // foreach valve that is still closed,
    // value = (30 - (time + dist[valve] + 1)) * flowRate[valve]
    int bestValue = 0;
    List<string> pathToBestValve = new List<string>();
    foreach (var valve in tunnels.Keys.Where(v => flowRates[v] > 0 && !openValves.Contains(v)))
    {
        var path = ShortestPath(loc, valve);
        var value = (30 - (time + path.Count)) * flowRates[valve];
        Console.WriteLine($"Could travel to {valve} (rate {flowRates[valve]}) in {path.Count - 1} steps, for an expected value of {value}");
        if (value > bestValue)
        {
            bestValue = value;
            pathToBestValve = path;
        }
    }
    if (pathToBestValve.Count > 0)
    {
        // execute!
        foreach (var step in pathToBestValve.Skip(1))
        {
            UpdateState();
            Console.WriteLine($"You move to {step}");
            loc = step;
        }

        UpdateState();
        string toOpen = pathToBestValve.Last();
        Console.WriteLine($"You open valve {toOpen}");
        openValves.Add(toOpen);
    }
    else
    {
        UpdateState();
    }
}
Console.WriteLine($"Total pressure released: {pressureReleased}");

void UpdateState()
{
    int releasing = openValves.Select(v => flowRates[v]).Sum();
    pressureReleased += releasing;
    Console.WriteLine($"\n== Minute {time} ==");
    time++;
    if (openValves!.Count == 0)
    {
        Console.WriteLine("No valves are open.");
    }
    else
    {
        Console.WriteLine($"Valves {string.Join(", ", openValves)} are open, releasing {releasing} pressure.");
    }
}

// Note that the sample plan _does_ require backtracking to II

List<string> ShortestPath(string src, string dst)
{
    var dist = tunnels!.Keys.ToDictionary(k => k, k => int.MaxValue);
    var prev = new Dictionary<string, string>();
    var q = new HashSet<string>(tunnels.Keys);
    dist[src] = 0;

    while (q.Count > 0)
    {
        string u = q.MinBy(x => dist[x])!;
        if (u == dst)
        {
            break;
        }
        q.Remove(u);
        foreach (var v in tunnels[u].Where(q.Contains))
        {
            var alt = dist[u] + 1;
            if (alt < dist[v])
            {
                dist[v] = alt;
                prev[v] = u;
            }
        }
    }

    var path = new List<string>();
    var w = dst;
    path.Add(w);
    while (prev.TryGetValue(w, out var v))
    {
        w = v;
        path.Add(w);
    }
    path.Reverse();
    return path;
}
