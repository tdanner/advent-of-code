var input = File.ReadAllText("input.txt");
for (int i = 3; i < input.Length; i++)
{
    char a = input[i - 3], b = input[i - 2], c = input[i - 1], d = input[i];
    if (a != b && a != c && a != d && b != c && b != d && c != d)
    {
        Console.WriteLine("start of packet: " + (i + 1));
        break;
    }
}

for (int i = 13; i < input.Length; i++)
{
    var chunk = input[(i - 13)..(i + 1)];
    if (chunk.Distinct().Count() == 14)
    {
        Console.WriteLine("start of message: " + (i + 1));
        break;
    }
}
