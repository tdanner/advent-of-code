string[] lines = File.ReadAllLines("input.txt");
int[] numbersDrawn = lines[0].Split(',').Select(line => int.Parse(line)).ToArray();
List<int[][]> boards = new();
for (int boardStartLine = 2; boardStartLine < lines.Length; boardStartLine += 6)
{
    int[][] board = new int[5][];
    for (int boardLine = 0; boardLine < 5; boardLine++)
    {
        board[boardLine] = lines[boardStartLine + boardLine]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToArray();
    }
    boards.Add(board);
}

bool BoardWins(int[][] board)
{
    for (int row = 0; row < 5; row++)
    {
        if (Enumerable.Range(0, 5).Select(column => board[row][column]).All(n => n == -1))
            return true;
    }

    for (int column = 0; column < 5; column++)
    {
        if (Enumerable.Range(0, 5).Select(row => board[row][column]).All(n => n == -1))
            return true;
    }

    return false;
}

int GetScore(int[][] board, int numberDrawn)
{
    return numberDrawn *
        board.SelectMany(b => b)
            .Where(n => n != -1)
            .Sum();
}

foreach (int numberDrawn in numbersDrawn)
{
    foreach (int[][] board in boards)
    {
        for (int row = 0; row < 5; row++)
        {
            for (int column = 0; column < 5; column++)
            {
                if (board[row][column] == numberDrawn)
                {
                    board[row][column] = -1;
                    if (BoardWins(board))
                    {
                        int winningScore = GetScore(board, numberDrawn);
                        Console.WriteLine(new { winningScore });
                        Environment.Exit(0);
                    }
                }
            }
        }
    }
}