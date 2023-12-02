using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input1.txt");
List<Game> games = lines.Select(ParseLine).ToList();
RGB ok = new RGB { r = 12, g = 13, b = 14 };

var answer = games.Where(g => g.Meets(ok)).Select(g => g.num).Sum();
Console.WriteLine("part 1: " + answer);
var part2 = games.Select(g => g.Power()).Sum();
Console.WriteLine("part 2: " + part2);

Game ParseLine(string line)
{
    int num = int.Parse(Regex.Match(line, @"Game (\d+):").Groups[1].Value);
    var rgbs = line.Substring(line.IndexOf(": ") + 2).Split("; ").Select(ParseRGB).ToList();
    return new Game { num = num, rgbs = rgbs };
}

RGB ParseRGB(string s)
{
    RGB rgb = new();
    foreach (string comp in s.Split(", "))
    {
        var comps = comp.Split(' ');
        int n = int.Parse(comps[0]);
        switch (comps[1])
        {
            case "red":
                rgb.r = n;
                break;
            case "green":
                rgb.g = n;
                break;
            case "blue":
                rgb.b = n;
                break;
        }
    }
    return rgb;
}

class Game
{
    public int num;
    public List<RGB> rgbs = [];
    public bool Meets(RGB rgb)
    {
        return rgbs.All(s => s.r <= rgb.r && s.g <= rgb.g && s.b <= rgb.b);
    }
    public override string ToString()
    {
        return "Game " + num + ": " + string.Join("; ", rgbs.Select(r => r.ToString()));
    }

    public int Power()
    {
        int r = rgbs.Select(rgb => rgb.r).Max();
        int g = rgbs.Select(rgb => rgb.g).Max();
        int b = rgbs.Select(rgb => rgb.b).Max();
        return r * g * b;
    }
}

struct RGB
{
    public int r, g, b;
    public override string ToString()
    {
        return $"{r} red, {g} green, {b} blue";
    }
}
