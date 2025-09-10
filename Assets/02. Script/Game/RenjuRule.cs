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
        private const int[,] board = new int[BOARD_SIZE, BOARD_SIZE];

        private static readonly int[,] directions =
        {
            { -1, -1 }, // 0: ↖ (대각선)
            { -1, 0 },  // 1: ↑ (세로)
            { -1, 1 },  // 2: ↗ (대각선)  
            { 0, -1 },  // 3: ← (가로)
            { 0, 1 },   // 4: → (가로)
            { 1, -1 },  // 5: ↙ (대각선)
            { 1, 0 },   // 6: ↓ (세로)
            { 1, 1 }    // 7: ↘ (대각선)
        };
        /// <summary>
        /// 렌주룰 적용 받는지
        /// 돌 번호 == 0: None, 1: Black, 2: White
        /// </summary>
        public static bool ImpossibleMove(board board, int row, int col)
        {
            if (playerType != 1) return false; // 흑돌만 적용

            var dir = directions;

            // 임시로 돌을 놓아서 체크
            board[row, col] = 1;

            bool result = false;

            // 임시 돌 제거
            board[row, col] = 0;

            return result;
        }
        
        // 우선 돌을 놓을 수 있는 곳인지 확인
        private static bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE;
        }
        
        // 한 방향으로 연속된 돌 개수 세기
        private static int CountStonesInDirection(board[,] board, int row, int col, int dx, int dy)
        {
            int count = 0;
            int x = row + dx;
            int y = col + dy;
    
            while (IsValidPosition(x, y) && board[x, y] == 1)
            {
                count++;
                x += dx;
                y += dy;
            }
    
            return count;
        }
        
        // 1. IsCloseThree를 통해 인접한 3의 개수를 구함
        public static bool Close33or44(board[,] board, int row, int col, int dir)
        {
            int threeCount = 0;
            int fourCount = 0;

            for (int i = 0; i < 4; i++)
            {
                int stones = 1; // 놓을 돌 포함
                
                // dir1
                int dx1 = directions[i, 0];
                int dy1 = directions[i, 1];
                // dir2 = 방향을 대칭으로 하기 위해 7 - i
                int dx2 = directions[7 - i, 0];
                int dy2 = directions[7 - i, 1];
                
                // dir1 체크
                int x = row + dx1;
                int y = col + dy1;
                //x, y의 좌표가 유효한 좌표인지 확인 && 값이 1(Black)인지 확인
                while (IsValidPosition(x, y) && board[x, y] == 1)
                {
                    // 맞다면 stone의 개수++ && 같은 방향으로 반복문
                    stones++;
                    x += dx1;
                    y += dy1;
                }
                
                // dir2
                x = row + dx2;
                y = col + dy2;
                while (IsValidPosition(x, y) && board[x, y] == 1)
                {
                    stones++;
                    x += dx1;
                    y += dy1;
                }
                if (stones >= 3)
                {
                    // 양 끝이 비어있는 지 체크
                    int frontX = row + dx1 * 2;
                    int frontY = col + dy1 * 2;
                    int backX = row + dx2 * 2;
                    int backY = col + dy2 * 2;
                
                    if (IsValidPosition(frontX, frontY) && board[frontX, frontY] == 0 &&
                        IsValidPosition(backX, backY) && board[backX, backY] == 0)
                    {
                        //비어 있다면
                        if (stones == 3) threeCount++;
                        if (stones == 4) fourCount++;
                    }
                }
                
                // 임시
                // if (stones == 3)
                // {
                //     // 양 끝이 비어있는 지 체크
                //     int frontX = row + dx1 * 2;
                //     int frontY = col + dy1 * 2;
                //     int backX = row + dx2 * 2;
                //     int backY = col + dy2 * 2;
                //
                //     if (IsValidPosition(frontX, frontY) && board[frontX, frontY] == 0 &&
                //         IsValidPosition(backX, backY) && board[backX, backY] == 0)
                //     {
                //         //비어 있다면
                //         threeCount++;
                //     }
                // }
                //
                // if (stones == 4)
                // {
                //     // 양 끝이 비어있는 지 체크
                //     int frontX = row + dx1 * 2;
                //     int frontY = col + dy1 * 2;
                //     int backX = row + dx2 * 2;
                //     int backY = col + dy2 * 2;
                //
                //     if (IsValidPosition(frontX, frontY) && board[frontX, frontY] == 0 &&
                //         IsValidPosition(backX, backY) && board[backX, backY] == 0)
                //     {
                //         //비어 있다면
                //         fourCount++;
                //     }
                // }
            }

            if (threeCount == 2 || fourCount == 2)
                return true;

            return false;
        }
        // 2. 인접해 있지 않은 특정 3-3 패턴
        // 2-1 흑돌을 놓았을 때 앞 O 뒤 OXXO -> OBOXXO && 반대로 (1, 2)
        // 2-2 흑돌을 놓았을 때 앞 OX 뒤 OXO -> OXBOXO && 반대로 (2, 1)
        
        // 3. 인접해 있지 않은 특정 4-4 패턴
        // 3-1 흑돌을 놓았을 때 앞 ? 뒤 OXXX? -> ?BOXXX? && 반대로 (1, 3)
        // 3-2 흑돌을 놓았을 때 앞 ?X 뒤 OXX? -> ?XBOXX? && 반대로 (2, 2)
        // 3-3 흑돌을 놓았을 때 앞 ?XX 뒤 OX? -> ?XBOXX? && 반대로 (3, 1)
        
        // 4. 굉장히 특수한 패턴
        // 4-1 XXBXXXO
        // 4-2 XXOBXOXX
    }
}
