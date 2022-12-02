internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var total = lines.Select(ParseLine).Select(game => Value(game.Item2) + Score(game)).Sum();
        Console.WriteLine(total);

        RPS Parse(char c) => c switch
        {
            'A' or 'X' => RPS.Rock,
            'B' or 'Y' => RPS.Paper,
            'C' or 'Z' => RPS.Scissors,
            _ => throw new Exception($"Unknown play {c}")
        };

        Tuple<RPS, RPS> ParseLine(string line) => Tuple.Create(Parse(line[0]), Parse(line[2]));

        int Value(RPS play) => play switch
        {
            RPS.Rock => 1,
            RPS.Paper => 2,
            RPS.Scissors => 3,
            _ => throw new Exception($"Unknown RPS {play}")
        };

        int Score(Tuple<RPS, RPS> game) => game switch
        {
            (RPS x, RPS y) when x == y => 3,
            (RPS.Rock, RPS.Paper) => 6,
            (RPS.Paper, RPS.Scissors) => 6,
            (RPS.Scissors, RPS.Rock) => 6,
            _ => 0
        };
    }
}

enum RPS { Rock, Paper, Scissors }
