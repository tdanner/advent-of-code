using System.Text.Json;
using System.Text.RegularExpressions;

internal static class Program
{
    static Dictionary<string, int> flowRates = new();
    static Dictionary<ulong, int> flowRateBits = new();
    static Dictionary<string, string[]> tunnels = new();
    static Dictionary<ulong, ulong[]> tunnelBits = new();
    static Dictionary<string, ulong> valveBits = new();
    static Dictionary<ulong, Dictionary<ulong, ulong>> directionsTo = new();
    static int numValvesToOpen;
    static ulong allValvesBits;

    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var parser = new Regex(@"Valve ([A-Z][A-Z]) has flow rate=(\d+); tunnels? leads? to valves? ([A-Z, ]+)");

        ulong valveBit = 1;
        foreach (var line in lines)
        {
            var m = parser.Match(line);
            var id = m.Groups[1].Value;
            flowRates[id] = int.Parse(m.Groups[2].Value);
            tunnels[id] = m.Groups[3].Value.Split(", ");
            valveBits[id] = valveBit;
            valveBit <<= 1;
        }
        Console.WriteLine(JsonSerializer.Serialize(flowRates));
        Console.WriteLine(JsonSerializer.Serialize(tunnels));
        numValvesToOpen = flowRates.Values.Count(f => f > 0);

        foreach (ulong valve in flowRateBits.Keys)
        {
            directionsTo[valve] = HowToGetTo(valve);
        }

        Console.WriteLine("graph");
        foreach (var (v, ts) in tunnels)
        {
            tunnelBits[valveBits[v]] = ts.Select(t => valveBits[t]).ToArray();
            flowRateBits[valveBits[v]] = flowRates[v];
            if (flowRates[v] > 0)
            {
                allValvesBits |= valveBits[v];
            }
            foreach (var t in ts)
            {
                string rate = flowRates[v] > 0 ? $" {flowRates[v]}" : "";
                Console.WriteLine($"\t{v}[{v}{rate}] --> {t}");
            }
        }

        // State start = new State("AA", new List<string>(), 1, 0, 0);
        // DFS(start);
        ulong aa = valveBits["AA"];
        State2 start = new State2(aa, aa, 0, 1, 0, 0, 0);
        DFS2(start);
    }

    static Dictionary<ulong, ulong> HowToGetTo(ulong dest)
    {
        var dist = tunnelBits!.Keys.ToDictionary(k => k, k => int.MaxValue);
        var prev = new Dictionary<ulong, ulong>();
        var q = new HashSet<ulong>(tunnelBits.Keys);
        dist[dest] = 0;

        while (q.Count > 0)
        {
            ulong u = q.MinBy(x => dist[x])!;
            q.Remove(u);
            foreach (var v in tunnelBits[u].Where(q.Contains))
            {
                var alt = dist[u] + 1;
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        return prev;
    }

    static int bestReleased = 0;
    static int pathsSearched = 0;

    static void DFS2(State2 here)
    {
        var options = here.GetNextStates().ToList();
        if (options.Count == 0)
        {
            pathsSearched++;
            if (here.Released > bestReleased)
            {
                bestReleased = here.Released;
                Console.WriteLine($"Got a final state with released {here.Released} and time {here.Time}");
            }
            if (pathsSearched % 10000000 == 0)
            {
                Console.WriteLine($"Searched {pathsSearched} paths; best = {bestReleased}");
            }
            return;
        }

        foreach (var next in options)
        {
            DFS2(next);
        }
    }

    record struct State2(ulong YLoc, ulong ELoc, ulong Open, int Time, int Released, ulong YVisited, ulong EVisited)
    {
        public int Releasing
        {
            get
            {
                int r = 0;
                for (int i = 0; i < valveBits.Count; i++)
                {
                    if ((Open & ((ulong)1 << i)) != 0)
                    {
                        r += flowRateBits[(ulong)1 << i];
                    }
                }
                return r;
            }
        }

        public IEnumerable<ulong> GetReasonableMoves(ulong start, ulong visited)
        {
            foreach (ulong next in tunnelBits[start])
            {
                if ((next & visited) == 0)
                {
                    yield return next;
                }
            }
        }

        public IEnumerable<State2> GetNextStates()
        {
            // we are going to iterate through all possible moves. no path planning!
            // that means cross-product of the number of tunnels Y and E can traverse + open the local valve 😬
            if (Time == 27)
            {
                yield break;
            }
            if (Open == allValvesBits)
            {
                yield return this with { Time = Time + 1, Released = Released + Releasing };
                yield break;
            }

            IEnumerable<ulong> eoptions = GetReasonableMoves(ELoc, EVisited);
            if (flowRateBits[ELoc] > 0 && ((Open & ELoc) == 0))
            {
                eoptions = eoptions.Append(0UL);
            }
            IEnumerable<ulong> yoptions = GetReasonableMoves(YLoc, YVisited);
            if (flowRateBits[YLoc] > 0 && ((Open & YLoc) == 0) && YLoc != ELoc) // if Y and E are on the save valve, only E opens it
            {
                yoptions = yoptions.Append(0UL);
            }
            int releasing = Releasing;
            foreach (var enext in eoptions)
            {
                foreach (var ynext in yoptions)
                {
                    var open = Open;
                    if (enext == 0UL)
                    {
                        open |= ELoc;
                    }
                    if (ynext == 0UL)
                    {
                        open |= YLoc;
                    }
                    yield return new State2(
                        ynext == 0UL ? YLoc : ynext,
                        enext == 0UL ? ELoc : enext,
                        open,
                        Time + 1,
                        Released + Releasing,
                        ynext == 0 ? 0 : YVisited | ynext,
                        enext == 0 ? 0 : EVisited | enext
                    );
                }
            }
        }
    }
}
