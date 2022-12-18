using System.Diagnostics;

internal static class Program
{
    private static void Main(string[] args)
    {
        var startTime = DateTime.Now;
        var winds = File.ReadAllText("input.txt");
        int t = 0;
        List<ushort> chamber = new(); // 0 is the bottom
        int topOfStack = 0, prevTop = 0;
        int by = 0, bx = 0;
        List<ushort> block = blocks[0];
        int blockNum = 0;
        List<int> growthPerCycle = new();

        for (blockNum = 0; blockNum < 1_000_000_000; blockNum++)
        {
            block = blocks[blockNum % blocks.Count];
            int blockWidth = blockWidths[blockNum % blocks.Count];
            bx = 2; // block always starts 2 spaces from the left
            by = topOfStack + 3; // block starts with its bottom 3 from the top of the stack
            while (chamber.Count < by + block.Count)
            {
                chamber.Add(0);
            }

            Print($"Block number {blockNum + 1} begins falling");

            while (true)
            {
                char wind = winds[t];
                if (wind == '<')
                {
                    if (bx > 0 && !Collides(-1, 0))
                    {
                        Print("Jet of gas pushes rock left:");
                        bx--;
                    }
                    else
                    {
                        Print("Jet of gas pushes rock left, but nothing happens:");
                    }
                }
                else if (wind == '>')
                {
                    if (bx + blockWidth < 7 && !Collides(1, 0))
                    {
                        bx++;
                        Print("Jet of gas pushes rock right:");
                    }
                    else
                    {
                        Print("Jet of gas pushes rock right, but nothing happens:");
                    }
                }
                t = (t + 1) % winds.Length;

                if (by == 0)
                {
                    break; // can't fall when already on the floor!
                }

                // test if falling one level would collide
                if (Collides(0, -1))
                {
                    break;
                }
                by--;
                Print("Rock falls 1 unit:");
            }
            // Apply the resting block to the chamber
            for (int y = 0; y < block.Count; y++)
            {
                chamber[y + by] |= (ushort)(block[y] << bx);
            }
            topOfStack = Math.Max(topOfStack, by + block.Count);

            Print("Rock falls 1 unit, causing it to come to rest:");
            // if (blockNum > 0 && blockNum % (blocks.Count * winds.Length) == 0)
            // {
            //     int diff = topOfStack - prevTop;
            //     growthPerCycle.Add(diff);
            //     prevTop = topOfStack;
            //     // Console.WriteLine();
            //     // Console.WriteLine(string.Join(", ", growthPerCycle.Select(p => p.ToString())));
            //     FindPeriodicity();
            //     // Console.WriteLine($"Top of stack is {topOfStack} after {blockNum + 1} blocks");
            // }
            if (blockNum % 1_000_000 == 0)
            {
                var elapsed = DateTime.Now - startTime;
                Console.WriteLine($"Dropped {blockNum / 1_000_000}M blocks in {blockNum / elapsed.TotalSeconds,0.00} blocks/sec");
            }
        }

        void FindPeriodicity()
        {
            Console.WriteLine($"Trying periods up to {growthPerCycle.Count / 2}");
            for (int period = 1; period < growthPerCycle.Count / 2; period++)
            {
                bool periodGood = true;
                for (int i = 0; periodGood && i < period; i++)
                {
                    for (int j = 0; j < growthPerCycle.Count; j += period)
                    {
                        if (growthPerCycle[i] != growthPerCycle[j])
                        {
                            periodGood = false;
                            break;
                        }
                    }
                }
                if (periodGood)
                {
                    Console.WriteLine($"Found period {period}!");
                    Console.WriteLine($"Every {period * blocks.Count * winds.Length} blocks");
                    Console.WriteLine($"The stack increases by {growthPerCycle.Take(period).Sum()} lines");
                    Console.WriteLine($"The stack increases by {growthPerCycle.Skip(100).Take(period).Sum()} lines");
                    // Environment.Exit(0);
                }
            }
        }

        bool Collides(int dx, int dy)
        {
            for (int y = 0; y < block.Count; y++)
            {
                ushort br = (ushort)(block[y] << (bx + dx));
                ushort cr = chamber[by + dy + y];
                if ((br & cr) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        const bool debug = false;
        void Print(string msg)
        {
            if (!debug || !msg.StartsWith("Block") /*&& blockNum != 2*/)
            {
                return;
            }
            Console.WriteLine(msg);
            for (int y = chamber.Count - 1; y >= 0; y--)
            {
                Console.Write('|');
                int bv = y - by;
                for (int x = 0; x < 7; x++)
                {
                    char p = '.';
                    if (bv >= 0 && bv < block.Count)
                    {
                        int bu = x - bx;
                        if (bu >= 0 && ((block[bv] & (1 << bu)) != 0))
                        {
                            p = '@';
                        }
                    }
                    if (y < chamber.Count && ((chamber[y] & (1 << x)) != 0))
                    {
                        p = '#';
                    }
                    Console.Write(p);
                }
                Console.WriteLine('|');
            }
            Console.WriteLine("+-------+");
            Console.WriteLine();
        }
    }

    static List<ushort> block1 = new() {
        0b1111,
    };
    static List<ushort> block2 = new(){
        0b010,
        0b111,
        0b010,
    };
    static List<ushort> block3 = new(){
        0b111,
        0b100,
        0b100,
    };
    static List<ushort> block4 = new(){
        0b1,
        0b1,
        0b1,
        0b1,
    };
    static List<ushort> block5 = new(){
        0b11,
        0b11,
    };
    static List<List<ushort>> blocks = new() { block1, block2, block3, block4, block5 };
    static List<int> blockWidths = new() { 4, 3, 3, 1, 2 };

}
