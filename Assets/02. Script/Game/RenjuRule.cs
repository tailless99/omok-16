using UnityEngine;
/// <summary>
/// 렌주룰 구현
/// 작업자 : 이승윤
/// </summary>
namespace Gomoku
{
    public class RenjuRule
    {
        private const int BOARD_SIZE = 15;

        private static readonly int[,] directions =
        {
            { -1, -1 }, { -1, 0 }, { -1, 1 },
            { 0, -1 }, { 0, 1 },
            { 1, -1 }, { 1, 0 }, { 1, 1 }
        };
/// <summary>
/// 렌주룰 적용 받는지 0: None, 1: Black, 2: White
/// </summary>
        public static bool ImpossibleMove(PlayerType playerType, board[,] board, int row, int col)
        {
            if (playerType != 1) return false; // 흑돌만 적용

            // 임시로 돌을 놓아서 체크
            board[row, col] = 1;
            
            bool result = IsCloseDoubleThree(board, row, col) ||
                          IsCloseDoubleFour(board, row, col) ||
                          IsOpenDoubleThree(board, row, col) ||
                          IsOpenDoubleFour(board, row, col) ||
                          IsSixOrMore(board, row, col);

            // 임시 돌 제거
            board[row, col] = 0;

            return result;
        }
        // 첫번째 조건 인접한 3-3
        public static bool IsCloseDoubleThree(board[,] board, int row, int col)
        {
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                
            }
        }
        
        // 두번째 조건 인접한 4-4
        public static bool IsCloseDoubleFour(board[,] board, int row, int col)
        {
            
        }
        
        // 세번째 조건 뛴 3-3
        public static bool IsOpenDoubleThree(board[,] board, int row, int col)
        {
            
        }
        
        // 네번째 조건 뛴 4-4
        public static bool IsOpenDoubleFour(board[,] board, int row, int col)
        {
            
        }
        
        // 다섯번째 조건 6개 이상
        public static bool IsSixOrMore(board[,] board, int row, int col)
        {
            
        }
    }
}
