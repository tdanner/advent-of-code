string[] lines = File.ReadAllLines("input.txt");
int bitCount = lines[0].Length;
int[] ones = new int[bitCount];
foreach (string line in lines) {
    for (int i = 0; i < bitCount; ++i) {
        if (line[i] == '1') {
            ones[i]++;
        }
    }
}

uint gamma = 0;
for (int i = 0; i < bitCount; ++i) {
    gamma <<= 1;
    if (ones[i] > lines.Length/2) {
        gamma++;
    }
}

uint mask = (((uint)1)<<bitCount) - 1;
uint epsilon = ~gamma & mask;

Console.WriteLine($"gamma: {gamma}, epsilon: {epsilon}, power: {gamma*epsilon}");
