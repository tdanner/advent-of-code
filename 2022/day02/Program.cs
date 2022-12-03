internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var total = lines.Select(ParseLine).Select(game => (int)DeterminePlay(game.Item1, game.Item2) + (int)game.Item2).Sum();
        Console.WriteLine(total);

        RPS ParseRPS(char c) => c switch
        {
            'A' or 'X' => RPS.Rock,
            'B' or 'Y' => RPS.Paper,
            'C' or 'Z' => RPS.Scissors,
            _ => throw new Exception($"Unknown play {c}")
        };

        Outcome ParseOutcome(char c) => c switch
        {
            'X' => Outcome.Lose,
            'Y' => Outcome.Draw,
            'Z' => Outcome.Win,
            _ => throw new Exception($"Unknown output {c}")
        };

        RPS DeterminePlay(RPS opponent, Outcome desired) => (opponent, desired) switch
        {
            (_, Outcome.Draw) => opponent,
            (RPS.Rock, Outcome.Win) => RPS.Paper,
            (RPS.Paper, Outcome.Win) => RPS.Scissors,
            (RPS.Scissors, Outcome.Win) => RPS.Rock,
            (RPS.Rock, Outcome.Lose) => RPS.Scissors,
            (RPS.Paper, Outcome.Lose) => RPS.Rock,
            (RPS.Scissors, Outcome.Lose) => RPS.Paper,
            _ => throw new Exception($"Unexpected ({opponent}, {desired})")
        };

        Tuple<RPS, Outcome> ParseLine(string line) => Tuple.Create(ParseRPS(line[0]), ParseOutcome(line[2]));
    }
}

enum RPS { Rock = 1, Paper = 2, Scissors = 3 }
enum Outcome { Win = 6, Lose = 0, Draw = 3 }
