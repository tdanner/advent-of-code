var input = File.ReadAllText("input.txt");
for (int i = 3; i < input.Length; i++)
{
    char a = input[i - 3], b = input[i - 2], c = input[i - 1], d = input[i];
    if (a != b && a != c && a != d && b != c && b != d && c != d)
    {
        Console.WriteLine(i + 1);
        break;
    }
}
