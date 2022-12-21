using System.Diagnostics;

var nums = File.ReadAllLines("input.txt").Select(short.Parse).ToArray();
var ll = new LinkedList<short>(nums);
var nodes = new List<LinkedListNode<short>>(ll.Count);
var n = ll.First;
while (n != null)
{
    nodes.Add(n);
    n = n.Next;
}
var len = nums.Length;

for (int i = 0; i < len; i++)
{
    Dump();
    int moves = nums[i];
    while (moves < 0)
        moves += nums.Length - 1;
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
Dump();

var zero = ll.Find(0)!;

int c1 = Seek(zero, 1000).Value;
int c2 = Seek(zero, 2000).Value;
int c3 = Seek(zero, 3000).Value;

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
    // Console.WriteLine(string.Join("\t", ll!.Select(n => n.ToString())));
}
