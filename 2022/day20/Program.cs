using System.Diagnostics;

const long DecryptionKey = 811589153;
var nums = File.ReadAllLines("input.txt").Select(long.Parse).Select(n => n * DecryptionKey).ToArray();
var ll = new LinkedList<long>(nums);
var nodes = new List<LinkedListNode<long>>(ll.Count);
var n = ll.First;
while (n != null)
{
    nodes.Add(n);
    n = n.Next;
}
var len = nums.Length;

for (int round = 0; round < 10; round++)
{
    for (int i = 0; i < len; i++)
    {
        // Dump();
        long moves = nums[i];
        moves %= (nums.Length - 1);
        moves += (nums.Length - 1);
        moves %= (nums.Length - 1);
        // Console.WriteLine($"{nums[i]} -> {moves}");
        if (moves == 0)
            continue;

        var src = nodes[i];
        Debug.Assert(nums[i] == src.Value);
        Debug.Assert(src.List != null);
        var dst = src;

        for (int j = 0; j < moves; j++)
        {
            if (dst!.Next == null)
                dst = ll.First;
            else
                dst = dst.Next;
        }
        ll.Remove(src);
        ll.AddAfter(dst!, src);
    }
    // Dump();
}

var zero = ll.Find(0)!;

long c1 = Seek(zero, 1000).Value;
long c2 = Seek(zero, 2000).Value;
long c3 = Seek(zero, 3000).Value;

Console.WriteLine($"{c1}, {c2}, {c3}, sum: {c1 + c2 + c3}");

LinkedListNode<T> Seek<T>(LinkedListNode<T> start, int dist)
{
    for (int i = 0; i < dist; i++)
    {
        if (start!.Next == null)
            start = start.List!.First!;
        else
            start = start.Next;
    }
    return start;
}


void Dump()
{
    Console.WriteLine(string.Join("\t", ll!.Select(n => n.ToString())));
}
