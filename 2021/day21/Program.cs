using System.Diagnostics;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string[] lines = File.ReadAllLines("input.txt");
        Dictionary<int, int> playerPositions = new();
        foreach (string line in lines)
        {
            Match m = Regex.Match(line, @"^Player (\d+) starting position: (\d+)$");
            Debug.Assert(m.Success);
            playerPositions[int.Parse(m.Groups[1].Value)] = int.Parse(m.Groups[2].Value);
        }

        Die die = new Die();
        Dictionary<int, int> playerScores = new();
        foreach (int player in playerPositions.Keys)
            playerScores[player] = 0;

        while (!playerScores.Values.Any(s => s >= 1000))
        {
            foreach (int player in playerPositions.Keys)
            {
                int[] rolls = new int[] { die.Roll(), die.Roll(), die.Roll() };
                int move = rolls.Sum();
                playerPositions[player] = (playerPositions[player] - 1 + move) % 10 + 1;
                playerScores[player] += playerPositions[player];
                // Console.WriteLine($"Player {player} rolls {rolls[0]}+{rolls[1]}+{rolls[2]} and moves to space {playerPositions[player]} for a total score of {playerScores[player]}.");
                if (playerScores[player] >= 1000)
                    break;
            }
        }

        Console.WriteLine(playerScores.Values.Min() * die.NumRolls);
    }
}

class Die
{
    int nextRoll = 0;
    public int NumRolls = 0;
    public int Roll()
    {
        NumRolls++;
        nextRoll %= 100;
        return ++nextRoll;
    }
}
