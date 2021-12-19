using System.Diagnostics;

static class Program
{
    static void Main()
    {
        FishNum[] nums = File.ReadAllLines("input.txt").Select(Parse).ToArray();
        FishNum sum = nums[0];
        foreach (var num in nums[1..])
        {
            sum = new Pair(sum, num);
            ((Pair)sum).Left.Parent = (Pair)sum;
            ((Pair)sum).Right.Parent = (Pair)sum;
            // Console.WriteLine(sum);
            Reduce(sum);
        }

        Console.WriteLine(sum.Magnitude());

        string[] lines = File.ReadAllLines("input.txt");
        long largest = 0;
        for (int i = 0; i < nums.Length; ++i)
        {
            for (int j = 0; j < nums.Length; ++j)
            {
                if (i == j) continue;
                long mag = Add(Parse(lines[i]), Parse(lines[j])).Magnitude();
                largest = Math.Max(largest, mag);
            }
        }
        Console.WriteLine(largest);
    }

    private static FishNum Add(FishNum a, FishNum b)
    {
        FishNum sum = new Pair(a, b);
        ((Pair)sum).Left.Parent = (Pair)sum;
        ((Pair)sum).Right.Parent = (Pair)sum;
        Reduce(sum);
        return sum;
    }

    private static void TestExplode(string before, string after)
    {
        FishNum num = Parse(before, 0, out int end);
        num.AddParentLinks();
        MaybeExplode(num, num, 0);
        Debug.Assert(num.ToString() == after);
    }

    static FishNum Parse(string str)
    {
        FishNum num = Parse(str, 0, out _);
        num.AddParentLinks();
        return num;
    }

    static FishNum Parse(string str, int start, out int end)
    {
        if (str[start] == '[')
        {
            FishNum left = Parse(str, start + 1, out int leftEnd);
            Debug.Assert(str[leftEnd] == ',');
            FishNum right = Parse(str, leftEnd + 1, out int rightEnd);
            Debug.Assert(str[rightEnd] == ']');
            end = rightEnd + 1;
            return new Pair(left, right);
        }
        else // Must be a regular number
        {
            Debug.Assert(char.IsDigit(str[start]));
            end = start + 1;
            return new RegularNum(str[start] - '0');
        }
    }

    static void Reduce(FishNum num)
    {
        while (MaybeExplode(num, num, 0) ||
               MaybeSplit(num, num))
        {
            // Console.WriteLine(num);
        }
    }

    private static void ValidateParents(FishNum num)
    {
        if (num is Pair pair)
        {
            Debug.Assert(pair.Left.Parent == pair);
            Debug.Assert(pair.Right.Parent == pair);
            ValidateParents(pair.Left);
            ValidateParents(pair.Right);
        }
    }

    static bool MaybeExplode(FishNum root, FishNum num, int depth)
    {
        if (num is Pair pair)
        {
            if (depth == 4)
            {
                RegularNum? nextLeft = FindNextLeft(root, pair.Left);
                if (nextLeft != null) nextLeft.Value += ((RegularNum)pair.Left).Value;
                RegularNum? nextRight = FindNextRight(root, pair.Right);
                if (nextRight != null) nextRight.Value += ((RegularNum)pair.Right).Value;

                Pair parent = pair.Parent!;
                if (pair == parent.Left)
                    parent.Left = new RegularNum(0) { Parent = pair.Parent };
                else
                    parent.Right = new RegularNum(0) { Parent = pair.Parent };

                return true;
            }
            else
            {
                return MaybeExplode(root, pair.Left, depth + 1) ||
                       MaybeExplode(root, pair.Right, depth + 1);
            }
        }
        else
        {
            return false;
        }
    }

    static bool MaybeSplit(FishNum root, FishNum num)
    {
        if (num is RegularNum reg && reg.Value >= 10)
        {
            var replacement = new Pair(new RegularNum(reg.Value / 2),
                                       new RegularNum(reg.Value - reg.Value / 2))
            { Parent = num.Parent };
            replacement.Left.Parent = replacement;
            replacement.Right.Parent = replacement;
            if (num == num.Parent!.Left)
                num.Parent.Left = replacement;
            else if (num == num.Parent!.Right)
                num.Parent.Right = replacement;

            return true;
        }
        else if (num is Pair pair)
        {
            return MaybeSplit(root, pair.Left) ||
                   MaybeSplit(root, pair.Right);
        }

        return false;
    }

    static IEnumerable<FishNum> InOrder(FishNum root)
    {
        if (root is RegularNum regular)
        {
            yield return regular;
        }
        else if (root is Pair pair)
        {
            foreach (var num in InOrder(pair.Left))
                yield return num;
            yield return pair;
            foreach (var num in InOrder(pair.Right))
                yield return num;
        }
    }

    static IEnumerable<FishNum> InOrderReverse(FishNum root) => InOrder(root).Reverse();

    private static RegularNum? FindNextLeft(FishNum root, FishNum pair)
    {
        RegularNum? prevReg = null;
        foreach (FishNum num in InOrder(root))
        {
            if (num == pair)
                return prevReg;

            if (num is RegularNum reg)
                prevReg = reg;
        }

        return null;
    }

    private static RegularNum? FindNextRight(FishNum root, FishNum pair)
    {
        RegularNum? prevReg = null;
        foreach (FishNum num in InOrderReverse(root))
        {
            if (num == pair)
                return prevReg;

            if (num is RegularNum reg)
                prevReg = reg;
        }

        return null;
    }
}

abstract class FishNum
{
    private Pair? _parent;
    public Pair? Parent
    {
        get => _parent;
        set
        {
            Debug.Assert(value != null);
            _parent = value;
        }
    }
    public abstract long Magnitude();

    public abstract void AddParentLinks();
}

class Pair : FishNum
{
    public FishNum Left, Right;
    public Pair(FishNum left, FishNum right)
    {
        Left = left;
        Right = right;
    }
    public override string ToString() => $"[{Left},{Right}]";
    public override long Magnitude() => 3 * Left.Magnitude() + 2 * Right.Magnitude();
    public override void AddParentLinks()
    {
        Left.Parent = this;
        Right.Parent = this;
        Left.AddParentLinks();
        Right.AddParentLinks();
    }
}

class RegularNum : FishNum
{
    public int Value;
    public RegularNum(int value)
    {
        Value = value;
    }
    public override string ToString() => Value.ToString();
    public override long Magnitude() => Value;
    public override void AddParentLinks() { }
}
