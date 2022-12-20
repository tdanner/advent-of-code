using System.Diagnostics;

var nums = File.ReadAllLines("sample.txt").Select(short.Parse).ToArray();
var len = nums.Length;
var mixed = new short[len];
var offset = new short[len];
Array.Copy(nums, mixed, len);
Console.Write("nums:\t");
Dump(mixed);

for (int i = 0; i < len; i++)
{
    var src = i + offset[i];
    Debug.Assert(mixed[src] == nums[i]); // make sure we are tracking offsets correctly
    var dst = (i + offset[i] + nums[i]) % len; // where are we going?
    if (src == dst) // no move? really?
    {
        // do nothing
    }
    if (src < dst) // moving forward. apply a -1 offset
    {
        Array.Copy(mixed, src + 1, mixed, src, dst - src);
        for (int j = 0; j < dst - src; j++)
        {
            offset[j]--;
        }
    }
    else // moving backward. apply a +1 offset
    {
        Array.Copy(mixed, src, mixed, src + 1, src - dst);
        for (int j = 0; j < src - dst; j++)
        {
            offset[j]++;
        }
    }
    mixed[dst] = nums[i];
    Console.Write("Mixed:\t");
    Dump(mixed);
    Console.Write("Offset:\t");
    Dump(offset);
}

void Dump(short[] a)
{
    Console.WriteLine(string.Join("\t", a));
}
