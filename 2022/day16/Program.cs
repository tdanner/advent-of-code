using System.Text.Json;
using System.Text.RegularExpressions;

internal class Program
{
    static Dictionary<string, int> flowRates = new();
    static Dictionary<string, string[]> tunnels = new();

    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var parser = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnels? leads? to valves? ([A-Z, ]+)");

        foreach (var line in lines)
        {
            var m = parser.Match(line);
            var id = m.Groups[1].Value;
            flowRates[id] = int.Parse(m.Groups[2].Value);
            tunnels[id] = m.Groups[3].Value.Split(", ");
        }
        Console.WriteLine(JsonSerializer.Serialize(flowRates));
        Console.WriteLine(JsonSerializer.Serialize(tunnels));

        Console.WriteLine("graph");
        foreach (var (v, ts) in tunnels)
        {
            foreach (var t in ts)
            {
                string rate = flowRates[v] > 0 ? $" {flowRates[v]}" : "";
                Console.WriteLine($"\t{v}[{v}{rate}] --> {t}");
            }
        }

        State start = new State("AA", new List<string>(), 1, 0, 0);
        DFS(start);
    }

    static List<string> ShortestPath(string src, string dst)
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

    static int bestReleased = 0;
    static int pathsSearched = 0;
    static void DFS(State here)
    {
        var options = here.GetNextStates().ToList();
        if (options.Count == 0)
        {
            int releasing = here.Releasing;
            for (int t = here.Time; t <= 30; t++)
            {
                here = here with { Time = t, Released = here.Released + releasing };
            }
            pathsSearched++;
            if (here.Released > bestReleased)
            {
                bestReleased = here.Released;
                Console.WriteLine($"Got a final state with released {here.Released} and time {here.Time}");
            }
            return;
        }

        options.Sort((s1, s2) => -s1.JustOpened.CompareTo(s2.JustOpened));
        foreach (var next in options)
        {
            DFS(next);
        }
    }

    record struct State(string Loc, List<string> Open, int Time, int Released, int JustOpened)
    {
        public int Releasing => Open.Select(v => flowRates[v]).Sum();
        public IEnumerable<State> GetNextStates()
        {
            var open = Open;
            // iterate through the valves that are not yet open and return a state for each one
            // that represents traveling to that valve and opening it
            foreach (var dest in tunnels.Keys.Where(v => flowRates[v] > 0 && !open.Contains(v)))
            {
                var path = ShortestPath(Loc, dest);
                if (Time + path.Count > 30)
                {
                    continue; // not a viable path
                }
                int releasing = Releasing;
                int released = Released;
                int time = Time;
                foreach (var step in path)
                {
                    time++;
                    released += releasing;
                }
                var nowOpen = new List<string>(open);
                nowOpen.Add(dest);
                yield return new State(dest, nowOpen, time, released, flowRates[dest]);
            }
        }
    }
}

// we are going to be searching though path space to find the best terminal state
// a terminal state has Time=30
// the best terminal state has the highest Flowed
// a state transition consists of travelling to another valve and opening it
// we will use recursion to explore the state space, which means states need to immutable
// would it be crazy to use a bitmask to represent the open valves? I don't think that would be crazy
