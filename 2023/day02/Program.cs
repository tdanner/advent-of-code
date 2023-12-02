using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input1.txt");
List<Game> games = lines.Select(ParseLine).ToList();
RGB ok = new RGB { r = 12, g = 13, b = 14 };

var answer = games.Where(g => g.Meets(ok)).Select(g => g.num).Sum();

foreach (Game g in games)
{
    if (g.Meets(ok))
    {
        Console.Write("Good:   ");
    }
    else
    {
        Console.Write("Bad :   ");
    }

    Console.WriteLine(g);
}

Console.WriteLine(answer);

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
}

struct RGB
{
    public int r, g, b;
    public override string ToString()
    {
        return $"{r} red, {g} green, {b} blue";
    }
}
