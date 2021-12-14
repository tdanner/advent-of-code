using System.Text;

string[] lines = File.ReadAllLines("input.txt");
string template = lines[0];
Dictionary<string, char> rules = lines[2..].ToDictionary(line => line[0..2], line => line[6]);

for (int step = 1; step <= 10; step++)
{
    StringBuilder polymer = new();
    for (int pos = 0; pos < template.Length - 1; pos++)
    {
        polymer.Append(template[pos]);
        polymer.Append(rules[template[pos..(pos + 2)]]);
    }
    polymer.Append(template[^1]);
    template = polymer.ToString();
    Console.WriteLine($"After step {step} polymer length is {polymer.Length}");
    // Console.WriteLine(template);
}

var histogram = template.GroupBy(c => c).OrderBy(g => g.Count()).ToList();
int mostFrequent = histogram[^1].Count();
int leastFrequent = histogram[0].Count();
Console.WriteLine(new { answer = mostFrequent - leastFrequent });