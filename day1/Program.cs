int previousValue = 1_000_000;
int increases = 0;
int[] values = File.ReadAllLines("input.txt").Select(line => int.Parse(line)).ToArray();
foreach (int value in values) {
    if (value > previousValue)
        increases++;
    previousValue = value;
}

Console.WriteLine($"{increases} increases");

int largerWindows = 0;
for (int i = 0; i < values.Length - 3; ++i) {
    int first = values[i] + values[i + 1] + values[i + 2];
    int second = values[i + 1] + values[i + 2] + values[i + 3];
    if (second > first)
        largerWindows++;
}

Console.WriteLine($"{largerWindows} larger windows");
