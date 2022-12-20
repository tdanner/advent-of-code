using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

static class Program
{
    static Regex parser = new Regex(@"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");
    public const int TimeLimit = 32;

    public static void Main()
    {
        var lines = File.ReadAllLines("input.txt");
        var blueprints = lines.Select(l => parser.Match(l)).Select(m =>
            new Blueprint(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value), int.Parse(m.Groups[5].Value), int.Parse(m.Groups[6].Value), int.Parse(m.Groups[7].Value))
        ).ToList();
        // Dump(blueprints);
        // int max = MaxGeodesBFS(blueprints[0]);
        long totalQuality = 0;
        foreach (var bp in blueprints.Take(3))
        {
            dfsMax = 0;
            var start = new State(0, 1, 0, 0, 0, 0, 0, 0, 0);
            int max = MaxGeodesDFS(bp, start);
            Console.WriteLine($"Blueprint {bp.id} gets max {max} geodes");
            totalQuality += bp.id * max;
        }
        Console.WriteLine($"Total quality: {totalQuality}");
    }

    public static int dfsMax = 0;

    private static int MaxGeodesBFS(Blueprint bp)
    {
        // going to try a BFS out to 24 rounds! I haven't done the math on this. it might take a trillion years.
        var start = new State(0, 1, 0, 0, 0, 0, 0, 0, 0);
        var q = new Queue<State>();
        q.Enqueue(start);
        int max = 0;
        int looked = 0;
        int finals = 0;
        int timeSeen = 0;
        while (q.Any())
        {
            var s = q.Dequeue();
            looked++;
            if (s.time > timeSeen)
            {
                Console.WriteLine($"Time {s.time}");
                timeSeen = s.time;
            }
            if (s.time == TimeLimit)
            {
                //Dump(s);
                max = Math.Max(max, s.geo);
                finals++;
            }
            else
            {
                // Dump(s, "options for: ");
                foreach (var next in s.Options(bp))
                {
                    // Dump(next);
                    q.Enqueue(next);
                }
            }
        }
        Console.WriteLine($"Looked at {looked} states and {finals} final states");
        return max;
    }

    private static int MaxGeodesDFS(Blueprint bp, State now)
    {
        if (now.time == TimeLimit)
        {
            if (now.geo > dfsMax)
            {
                dfsMax = now.geo;
                // if (dfsMax == 8)
                // {
                //     Console.WriteLine(now.story);
                //     Debugger.Break();
                // }
                Console.WriteLine($"Max so far: {dfsMax}");
            }
            return now.geo;
        }

        int max = 0;
        foreach (var next in now.Options(bp))
        {
            max = Math.Max(max, MaxGeodesDFS(bp, next));
        }
        return max;
    }

    private static void Dump<T>(T obj, string prefix = "")
    {
        Console.WriteLine(prefix + JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));
    }
}

record struct State(byte time, byte oreBots, byte clayBots, byte obsBots, byte geoBots,
byte ore, byte clay, byte obs, byte geo
//, Story? story
)
{
    public IEnumerable<State> Options(Blueprint bp)
    {
        int timeLeft = (Program.TimeLimit - time);
        int maxPossibleGeodes = geo + geoBots * timeLeft + timeLeft * (timeLeft + 1) / 2;
        if (maxPossibleGeodes < Program.dfsMax)
        {
            yield break; // give up - can't possibly win
        }

        // the existing robots always gather their stuff - these are always the same
        // the options are basically make one of each robot or make no robots
        // is there ever a scenario where you would build two robots in one minute? I hope not.
        // -- no: there is only one robot factory and it takes one minute to make a robot
        var doNothing = this with
        {
            time = (byte)(time + 1),
            ore = (byte)(ore + oreBots),
            clay = (byte)(clay + clayBots),
            obs = (byte)(obs + obsBots),
            geo = (byte)(geo + geoBots),
        };

        // ** make a geode bot
        if (time < Program.TimeLimit && ore >= bp.geoBotOre && obs >= bp.geoBotObs)
        {
            yield return doNothing with
            {
                ore = (byte)(doNothing.ore - bp.geoBotOre),
                obs = (byte)(doNothing.obs - bp.geoBotObs),
                geoBots = (byte)(geoBots + 1),
            };
        }

        // ** make an obsidian bot
        if (time < Program.TimeLimit - 1 && ore >= bp.obsBotOre && clay >= bp.obsBotClay && obsBots < bp.MaxObsBots)
        {
            yield return doNothing with
            {
                ore = (byte)(doNothing.ore - bp.obsBotOre),
                clay = (byte)(doNothing.clay - bp.obsBotClay),
                obsBots = (byte)(obsBots + 1),
            };
        }

        // ** make a clay bot
        if (time < Program.TimeLimit - 1 && ore >= bp.clayBotOre && clayBots < bp.MaxClayBots)
        {
            yield return doNothing with
            {
                ore = (byte)(doNothing.ore - bp.clayBotOre),
                clayBots = (byte)(clayBots + 1),
            };
        }

        // ** make an ore bot
        if (time < Program.TimeLimit - 1 && ore >= bp.oreBotOre && oreBots < bp.MaxOreBots)
        {
            yield return doNothing with
            {
                ore = (byte)(doNothing.ore - bp.oreBotOre),
                oreBots = (byte)(oreBots + 1),
            };
        }

        yield return doNothing;
    }
}

record class Blueprint(int id, int oreBotOre, int clayBotOre, int obsBotOre, int obsBotClay, int geoBotOre, int geoBotObs)
{
    public int MaxOreBots { get; init; } = new[] { oreBotOre, clayBotOre, obsBotOre, geoBotOre }.Max();
    public int MaxClayBots { get; init; } = new[] { obsBotClay }.Max();
    public int MaxObsBots { get; init; } = new[] { geoBotObs }.Max();
}

record class Story(string episode, Story? before)
{
    public override string ToString()
    {
        string s = before?.ToString() ?? "";
        return s + episode;
    }
}
