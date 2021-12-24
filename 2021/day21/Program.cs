using System.Diagnostics;
using System.Text.RegularExpressions;
using TimDanner.Utils;

class Program
{
    static Dictionary<int, int> playerPositions = new();

    static void Main()
    {
        string[] lines = File.ReadAllLines("input.txt");
        foreach (string line in lines)
        {
            Match m = Regex.Match(line, @"^Player (\d+) starting position: (\d+)$");
            Debug.Assert(m.Success);
            playerPositions[int.Parse(m.Groups[1].Value)] = int.Parse(m.Groups[2].Value);
        }

        // FirstPart();
        SecondPart();
    }

    static void FirstPart()
    {
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

    static void SecondPart()
    {
        Counter<Universe> multiverse = new();
        multiverse[new Universe(new Player(playerPositions[1], 0),
                                new Player(playerPositions[2], 0),
                                null)] = 1;

        bool player1sTurn = true;
        while (multiverse.Keys.Any(uni => uni.winner == null))
        {
            Console.WriteLine($"num universes: {multiverse.Keys.Count()}");
            Counter<Universe> nextTurn = new();
            foreach (Universe uni in multiverse.Keys)
            {
                if (uni.winner != null)
                {
                    nextTurn[uni] += multiverse[uni];
                    continue;
                }

                for (int roll1 = 1; roll1 <= 3; roll1++)
                {
                    for (int roll2 = 1; roll2 <= 3; roll2++)
                    {
                        for (int roll3 = 1; roll3 <= 3; roll3++)
                        {
                            int move = roll1 + roll2 + roll3;
                            Player pBefore = player1sTurn ? uni.p1 : uni.p2;
                            int position = (pBefore.position - 1 + move) % 10 + 1;
                            Player pAfter = new(position, pBefore.score + position);
                            int? winner = null;
                            if (pAfter.score >= 21)
                                winner = player1sTurn ? 1 : 2;

                            Universe uniAfter = player1sTurn ?
                                new Universe(pAfter, uni.p2, winner) :
                                new Universe(uni.p1, pAfter, winner);

                            nextTurn[uniAfter] += multiverse[uni];
                        }
                    }
                }
            }
            // 414036821918489
            // 217503477215922
            // 
            multiverse = nextTurn;
            player1sTurn = !player1sTurn;
        }

        Console.WriteLine(new
        {
            player1wins = multiverse.Where(uni => uni.Key.winner == 1).Sum(uni => uni.Value),
            player2Wins = multiverse.Where(uni => uni.Key.winner == 2).Sum(uni => uni.Value)
        });
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

record Player(int position, int score);
record Universe(Player p1, Player p2, int? winner);