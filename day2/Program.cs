string[] lines = File.ReadAllLines("input.txt");
int position = 0;
int depth = 0;
foreach (string line in lines) {
    string[] parts = line.Split(' ');
    string direction = parts[0];
    int amount = int.Parse(parts[1]);
    switch (direction) {
        case "forward":
            position += amount;
            break;
        case "down":
            depth += amount;
            break;
        case "up":
            depth -= amount;
            break;
        default:
            throw new ArgumentOutOfRangeException("direction", direction);
    }
}

Console.WriteLine($"position: {position}, depth: {depth}, product: {position * depth}");
