var expenses = File.ReadAllLines("input.txt").Select(long.Parse).ToArray();
foreach (var e1 in expenses)
{
    foreach (var e2 in expenses)
    {
        foreach (var e3 in expenses)
        {
            if (e1 + e2 + e3 == 2020)
            {
                Console.WriteLine(e1 * e2 * e3);
                break;
            }
        }
    }
}