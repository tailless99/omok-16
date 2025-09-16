using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 렌주룰 구현
/// 작업자 : 이승윤
/// </summary>
namespace Gomoku
{
    public static class RenjuRule
    {
        private const int BOARD_SIZE = 15;

        private static readonly int[,] directions =
        {
            { -1, -1 }, // 0: ↖ (대각선)
            { -1, 0 },  // 1: ↑ (세로)
            { -1, 1 },  // 2: ↗ (대각선)  
            { 0, -1 },  // 3: ← (가로)
            // 아래 4개는 위 방향들의 정반대 방향
            { 1, 1 },   // 4: ↘ (대각선)
            { 1, 0 },   // 5: ↓ (세로)
            { 1, -1 },  // 6: ↙ (대각선)
            { 0, 1 }    // 7: → (가로)
        };

        /// <summary>
        /// 흑돌의 금수(3-3, 4-4, 장목)인지 검사
        /// player: 0: None, 1: Black, 2: White
        /// </summary>
        public static bool IsForbiddenMove(int[,] board, int row, int col, int player)
        {
            if (player != 1) return false; // 흑돌만 렌주룰 적용

            // 임시로 돌을 놓아서 금수인지 체크
            board[row, col] = player;

            bool isForbidden = Is33Or44(board, row, col, player) || 
							   IsOverline(board, row, col, player);

            // 임시로 놓았던 돌 제거
            board[row, col] = 0;

            return isForbidden;
        }

        public static List<(int row, int col)> FindAllForbiddenMoves(int[,] board, int player)
        {
            var forbiddenMoves = new List<(int row, int col)>();
            
            for (int r = 0; r < BOARD_SIZE; r++)
            {
                for (int c = 0; c < BOARD_SIZE; c++)
                {
                    if (board[r, c] != 0) continue;
                    
                    if (IsForbiddenMove(board, r, c, player))
                    {
                        forbiddenMoves.Add((r, c));
                    }
                }
            }

            return forbiddenMoves;
        }
        
        // 금수 위치를 표시할 보드 업데이트
        public static void UpdateForbiddenMarkers(int[,] board, int player, bool[,] forbiddenMarkersBoard)
        {
            // 1. 기존의 금수 마크를 모두 초기화합니다.
            System.Array.Clear(forbiddenMarkersBoard, 0, forbiddenMarkersBoard.Length);

            if (player != 1) return;
            // 2. 모든 금수 위치를 찾습니다.
            List<(int row, int col)> forbiddenMoves = FindAllForbiddenMoves(board, player);

            // 3. 찾은 위치를 forbiddenMarkersBoard에 true로 표시합니다.
            foreach (var move in forbiddenMoves)
            {
                forbiddenMarkersBoard[move.row, move.col] = true;
            }
        }


        /// <summary>
        /// 6목 이상(장목)인지 검사
        /// </summary>
        private static bool IsOverline(int[,] board, int row, int col, int player)
        {
            for (int i = 0; i < 4; i++) // 4가지 방향 축(가로, 세로, 대각선 2개)만 검사
            {
                int count = 1;
                count += CountStonesInDirection(board, row, col, directions[i, 0], directions[i, 1], player);
                count += CountStonesInDirection(board, row, col, directions[i + 4, 0], directions[i + 4, 1], player);

                if (count > 5) return true;
            }
            return false;
        }
        
        private static int CountStonesInDirection(int[,] board, int row, int col, int dRow, int dCol, int player)
        {
            int count = 0;
            int r = row + dRow;
            int c = col + dCol;
            while (IsValidPosition(r, c) && board[r, c] == player)
            {
                count++;
                r += dRow;
                c += dCol;
            }
            return count;
        }


        /// <summary>
        /// 3-3 또는 4-4인지 검사
        /// 연속된 돌에 대한 '열린 3'과 '열린 4'를 검사하며,
        /// </summary>
        private static bool Is33Or44(int[,] board, int row, int col, int player)
        {
            int openThreeCount = 0;
            int openFourCount = 0;

            for (int i = 0; i < 4; i++) // 4가지 방향 축
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];

                // B를 중심으로 양쪽 5칸씩, 총 11칸의 라인을 문자열로 만들고
                // 문자열 비교를 통해 금지된 패턴을 찾음
                var lineBuilder = new System.Text.StringBuilder();
                for (int k = 5; k >= 1; k--)
                {
                    int r = row - k * dx;
                    int c = col - k * dy;
                    AppendBoardState(lineBuilder, board, r, c, player);
                }

                lineBuilder.Append('B'); // 현재 놓는 돌

                for (int k = 1; k <= 5; k++)
                {
                    int r = row + k * dx;
                    int c = col + k * dy;
                    AppendBoardState(lineBuilder, board, r, c, player);
                }

                string line = lineBuilder.ToString();
                string tempLine = line.Replace('B', 'X'); // B를 X로 취급하여 패턴 검사

                // 열린 3 검사 (B가 놓아져서 완성되는 경우)
                // 1. 열린 연속된 3, 2. 중간이 비어있는 열린 3
                if (tempLine.Contains("_XXX_") || tempLine.Contains("_X_XX_") || tempLine.Contains("_XX_X_"))
                {
                    if (tempLine.Contains("_X_XXX_") || tempLine.Contains("_XXX_X_") ||
                        tempLine.Contains("W_XXX_") || tempLine.Contains("_XXX_W"))
                        continue;
                    
                    openThreeCount++;
                    
                    Debug.Log($"openThreeCount : {openThreeCount}");
                }

                // 열린 4 검사 (B가 놓아져서 완성되는 경우)
                if (tempLine.Contains("_XXXX_") || tempLine.Contains("_X_XXX_") || tempLine.Contains("_XX_XX_") || tempLine.Contains("_XXX_X_") ||
                    // 한쪽이 상대 돌 'O' 또는 벽 'W'로 막힌 4의 모든 경우 추가
                    tempLine.Contains("_XXXXO") || tempLine.Contains("OXXXX_") ||
                    tempLine.Contains("OX_XXX_") || tempLine.Contains("_XXX_XO") ||
                    tempLine.Contains("OXX_XX_") || tempLine.Contains("_XX_XXO") ||
                    tempLine.Contains("OXXX_X_") || tempLine.Contains("_X_XXXO") ||
                    tempLine.Contains("_XXXXW") || tempLine.Contains("WXXXX_") ||
                    tempLine.Contains("_X_XXXW") || tempLine.Contains("WX_XXX_") ||
                    tempLine.Contains("_XX_XXW") || tempLine.Contains("WXX_XX_") ||
                    tempLine.Contains("_XXX_XW") || tempLine.Contains("WXXX_X_")
                   )
                {
                    openFourCount++;
                    Debug.Log($"openFourCount : {openFourCount}");
                }

                if (tempLine.Contains("XXXXXX_") || tempLine.Contains("XX_XX_XX"))
                {
                    openFourCount = 2;
                }
            }

            return openThreeCount >= 2 || openFourCount >= 2;

        }

        // 돌을 놓을 수 있는 유효한 위치인지 확인
        private static bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
        }
        
        // 라인 문자열을 만들기 위한 헬퍼 메서드
        private static void AppendBoardState(System.Text.StringBuilder builder, int[,] board, int r, int c, int player)
        {
            if (!IsValidPosition(r, c))
            {
                builder.Append('W'); // Wall (보드 밖)
            }
            else if (board[r, c] == 0)
            {
                builder.Append('_'); // Empty
            }
            else if (board[r, c] == player)
            {
                builder.Append('X'); // Player's stone
            }
            else
            {
                builder.Append('O'); // Opponent's stone
            }
        }


        // 2. 인접해 있지 않은 특정 3-3 패턴
        // 2-1 흑돌을 놓았을 때 앞 _ 뒤 _XX_ -> _B_XX_ && 반대로 (1, 2)
        // 2-2 흑돌을 놓았을 때 앞 _X 뒤 _X_ -> _XB_X_ && 반대로 (2, 1)
        
        // 3. 인접해 있지 않은 특정 4-4 패턴
        // 3-1 흑돌을 놓았을 때 앞 ? 뒤 _XXX? -> ?B_XXX? && 반대로 (1, 3)
        // 3-2 흑돌을 놓았을 때 앞 ?X 뒤 _XX? -> ?XB_XX? && 반대로 (2, 2)
        // 3-3 흑돌을 놓았을 때 앞 ?XX 뒤 _X? -> ?XB_XX? && 반대로 (3, 1)
        
        // 4. 굉장히 특수한 패턴
        // 4-1 XXBXXX_
        // 4-2 XX_BX_XX
    }
}
