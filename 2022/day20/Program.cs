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
Dump();

for (int i = 0; i < len; i++)
{
    if (nums[i] == 0)
        continue;
    var src = nodes[i];
    Debug.Assert(nums[i] == src.Value);
    Debug.Assert(src.List != null);
    var dst = src;
    if ((nums[i] + len * 2) % len == 0)
    {
        continue;
    }
    if (nums[i] > 0)
    {
        for (int j = 0; j < nums[i]; j++)
        {
            if (dst!.Next == null)
                dst = ll.First;
            else
                dst = dst.Next;
        }
        ll.Remove(src);
        ll.AddAfter(dst!, src);
    }
    else if (nums[i] < 0)
    {
        for (int j = nums[i]; j < 0; j++)
        {
            if (dst!.Previous == null)
                dst = ll.Last;
            else
                dst = dst.Previous;
        }
        ll.Remove(src);
        ll.AddBefore(dst!, src);
    }
    Dump();
}

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
    foreach (var n in ll)
    {
        //Console.Write("\t" + n);
    }
    // Console.WriteLine();
}
