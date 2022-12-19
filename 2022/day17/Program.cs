using System.Diagnostics;
using System.Text.Json;

internal static class Program
{
    record struct State(int windIdx, int blockIdx, byte top, byte second);
    record struct Height(int height, long bnum);
    private static void Main(string[] args)
    {
        var startTime = DateTime.Now;
        var winds = File.ReadAllText("input.txt");
        int t = 0;
        List<byte> chamber = new(); // 0 is the bottom
        int topOfStack = 0;
        int by = 0, bx = 0;
        List<byte> block = blocks[0];
        long blockNum = 0;
        List<int> growthPerCycle = new();
        Dictionary<State, Height> heights = new();
        long heightCreatedByRepeats = 0;

        for (blockNum = 0; blockNum < 1_000_000_000_000; blockNum++)
        {
            block = blocks[(int)(blockNum % blocks.Count)];
            int blockWidth = blockWidths[(int)(blockNum % blocks.Count)];
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
                chamber[y + by] |= (byte)(block[y] << bx);
            }
            topOfStack = Math.Max(topOfStack, by + block.Count);

            var shape2string = (byte s) => Convert.ToString((byte)s, 2).PadLeft(7, '0');

            if (heightCreatedByRepeats == 0 && topOfStack > 2)
            {
                byte topShape = chamber[topOfStack - 1];
                byte secondShape = chamber[topOfStack - 2];
                if ((topShape | secondShape) == 0b1111111)
                {
                    State s = new State(t, (int)(blockNum % blocks.Count), topShape, secondShape);
                    if (heights.TryGetValue(s, out Height h))
                    {
                        //Console.WriteLine($"State has repeated after growing {topOfStack - h.height} lines with {blockNum - h.bnum} blocks: {s}");
                        long blocksRemaining = 1_000_000_000_000 - blockNum;
                        long remainderAfterRepeats = blocksRemaining % (blockNum - h.bnum);
                        long repeatsNeeded = blocksRemaining / (blockNum - h.bnum);
                        heightCreatedByRepeats = repeatsNeeded * (topOfStack - h.height);
                        long finalHeight = topOfStack + heightCreatedByRepeats;
                        Console.WriteLine(JsonSerializer.Serialize(new
                        {
                            h,
                            blockNum,
                            topOfStack,
                            remainderAfterRepeats,
                            repeatsNeeded,
                            heightCreatedByRepeats,
                            finalHeight
                        }, new JsonSerializerOptions { WriteIndented = true }));

                        blockNum += (blockNum - h.bnum) * repeatsNeeded;

                    }
                    heights[s] = new Height(topOfStack, blockNum);
                    Console.WriteLine($"Tracking {heights.Count} states, blocknum={blockNum}");
                    // string v = Convert.ToString((byte)topShape, 2);
                    // v = v.PadLeft(7, '0');
                    // Console.WriteLine($"Block #{blockNum} top of stack {shape2string(topShape)},{shape2string(secondShape)}");
                    // Thread.Sleep(100);
                }
            }
            Print("Rock falls 1 unit, causing it to come to rest:");
        }
        Console.WriteLine($"Part 2: {topOfStack + heightCreatedByRepeats}");

        bool Collides(int dx, int dy)
        {
            for (int y = 0; y < block.Count; y++)
            {
                byte br = (byte)(block[y] << (bx + dx));
                byte cr = chamber[by + dy + y];
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

    static List<byte> block1 = new() {
        0b1111,
    };
    static List<byte> block2 = new(){
        0b010,
        0b111,
        0b010,
    };
    static List<byte> block3 = new(){
        0b111,
        0b100,
        0b100,
    };
    static List<byte> block4 = new(){
        0b1,
        0b1,
        0b1,
        0b1,
    };
    static List<byte> block5 = new(){
        0b11,
        0b11,
    };
    static List<List<byte>> blocks = new() { block1, block2, block3, block4, block5 };
    static List<int> blockWidths = new() { 4, 3, 3, 1, 2 };
}
