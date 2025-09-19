
public class TurnState
{
    public int[,] boardState;
    public int turnNumber;
    public int currPlayer;
    public int row;
    public int col;

    public TurnState(int[,] board, int turn, int player, int row, int col)
    {
        boardState = board;

        for (int x = 0; x < Constants.BlockColumnCount; x++)
        {
            for (int y = 0; y < Constants.BlockColumnCount; y++)
            {
                boardState[x, y] = board[x, y];
            }
        }
        
        turnNumber = turn;
        currPlayer = player;
        this.row = row;
        this.col = col;
    }
}
