using UnityEngine;

/// <summary>
/// 작성자 : 김동건
/// 오목 AI 알고리즘을 구현하기 위한 스크립트
/// </summary>
public static class OmokAI
{
    private static int searchPadding = 1; // 주변 몇 칸까지 탐색할지 설정
    private static int maxCalculation = 1000000; // DoMiniMax 함수의 최대 호출 횟수 제한을 위한 변수
    private static int currCalculation = 0; // DoMiniMax 함수의 현재 호출 횟수
    
    /// <summary>
    /// 작성자 : 김동건
    /// 현재 보드 상태를 전달하면 다음 최적의 좌표를 반환하는 함수
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public static (int row, int col)? GetBestMove(Constants.PlayerType[,] board)
    {
        float bestScore = -1000;
        (int row, int col) movePosition = (-1, -1);
        
        // 연산을 줄이기 위해 돌이 있는 곳의 영역 좌표를 구함
        int minAbleRow = board.GetLength(0);
        int maxAbleRow = -1;
        int minAbleCol = board.GetLength(1);
        int maxAbleCol = -1;
        
        bool isFirstTurn = true; // 돌이 하나라도 있는지 확인

        for (int row = 0; row < board.GetLength(0); row++)
        {
            for (int col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] != Constants.PlayerType.None)
                {
                    minAbleRow = Mathf.Min(minAbleRow, row);
                    maxAbleRow = Mathf.Max(maxAbleRow, row);
                    minAbleCol = Mathf.Min(minAbleCol, col);
                    maxAbleCol = Mathf.Max(maxAbleCol, col);
                    isFirstTurn = false;
                }
            }
        }

        
        int startRow, endRow, startCol, endCol;

        if (isFirstTurn) // 처음 둔다면 중앙 근처만 영역으로 지정
        {
            startRow = board.GetLength(0) / 2 - 2; 
            endRow = board.GetLength(0) / 2 + 2;
            startCol = board.GetLength(1) / 2 - 2; 
            endCol = board.GetLength(1) / 2 + 2;
        }
        else // 아니라면 돌이 있는 영역 + searchPadding을 영역으로 지정
        {
            startRow = Mathf.Max(0, minAbleRow - searchPadding);
            endRow = Mathf.Min(board.GetLength(0) - 1, maxAbleRow + searchPadding);
            startCol = Mathf.Max(0, minAbleCol - searchPadding);
            endCol = Mathf.Min(board.GetLength(1) - 1, maxAbleCol + searchPadding);
        }
        
        // 위 조건문으로 제한한 영역만 탐색
        for (var row = startRow; row <= endRow; row++)
        {
            for (var col = startCol; col <= endCol; col++)
            {
                if (board[row, col] == Constants.PlayerType.None)
                {
                    board[row, col] = Constants.PlayerType.PlayerB;
                    var score = OmokAI.DoMiniMax(board, 0, false, float.MinValue, float.MaxValue);
                    board[row, col] = Constants.PlayerType.None;

                    if (score > bestScore) // minimax를 사용하여 최적의 좌표 반환
                    {
                        bestScore = score;
                        movePosition = (row, col);
                    }
                }
            }
        }

        if (movePosition != (-1, -1))
        {
            return (movePosition.row, movePosition.col);
        }

        return null;
    }

    /// <summary>
    /// 작성자 : 김동건
    /// depth만큼 가상의 수를 두어 점수로 반환하는 함수
    /// </summary>
    /// <param name="board"></param>
    /// <param name="depth"></param>
    /// <param name="isMaximizing"></param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    private static float DoMiniMax(Constants.PlayerType[,] board, int depth, bool isMaximizing, float alpha, float beta)
    {
        // 게임 종료 상태 체크
        if (CheckGameWin(Constants.PlayerType.PlayerA, board))
            return -225 + depth;
        if (CheckGameWin(Constants.PlayerType.PlayerB, board))
            return 225 - depth;
        if (CheckGameDraw(board)|| depth >= 3 || currCalculation >= maxCalculation) // depth 제한, 호출 횟수 제한 (난이도 조절의 핵심)
            return 0;
        
        // 연산을 줄이기 위해 돌이 있는 곳의 영역 좌표를 구함
        int minAbleRow = board.GetLength(0);
        int maxAbleRow = -1;
        int minAbleCol = board.GetLength(1);
        int maxAbleCol = -1;
        
        bool isFirstTurn = true; // 보드에 돌이 하나라도 있는지 확인

        int boardRows = board.GetLength(0);
        int boardCols = board.GetLength(1);

        for (int row = 0; row < boardRows; row++)
        {
            for (int col = 0; col < boardCols; col++)
            {
                if (board[row, col] != Constants.PlayerType.None)
                {
                    minAbleRow = Mathf.Min(minAbleRow, row);
                    maxAbleRow = Mathf.Max(maxAbleRow, row);
                    minAbleCol = Mathf.Min(minAbleCol, col);
                    maxAbleCol = Mathf.Max(maxAbleCol, col);
                    isFirstTurn = false;
                }
            }
        }
        
        int startRow, endRow, startCol, endCol;

        if (isFirstTurn) // 처음 둔다면 중앙 근처만 영역으로 지정
        {
            int centerRow = boardRows / 2;
            int centerCol = boardCols / 2;
            
            startRow = Mathf.Max(0, centerRow - 2);
            endRow = Mathf.Min(boardRows - 1, centerRow + 2);
            startCol = Mathf.Max(0, centerCol - 2);
            endCol = Mathf.Min(boardCols - 1, centerCol + 2);
        }
        else // 아니라면 돌이 있는 영역 + searchPadding을 영역으로 지정
        {
            startRow = Mathf.Max(0, minAbleRow - searchPadding);
            endRow = Mathf.Min(boardRows - 1, maxAbleRow + searchPadding);
            startCol = Mathf.Max(0, minAbleCol - searchPadding);
            endCol = Mathf.Min(boardCols - 1, maxAbleCol + searchPadding);
        }
        

        if (isMaximizing) // AI(최대화 플레이어) 차례
        {
            var bestScore = float.MinValue;
            
            // 위 조건문으로 제한한 영역만 탐색
            for (var row = startRow; row <= endRow; row++) 
            {
                for (var col = startCol; col <= endCol; col++) 
                {
                    if (board[row, col] == Constants.PlayerType.None)
                    {
                        board[row, col] = Constants.PlayerType.PlayerB;
                        var score = DoMiniMax(board, depth + 1, false, alpha, beta);
                        board[row, col] = Constants.PlayerType.None;

                        bestScore = Mathf.Max(score, bestScore);
                        alpha = Mathf.Max(alpha, bestScore);

                        if (alpha >= beta) // 알파-베타 가지치기
                        {
                            break; // 안쪽 for 루프 탈출
                        }
                    }
                }
                if (alpha >= beta) // 알파-베타 가지치기
                {
                    break; // 바깥쪽 for 루프도 탈출
                }
            }
            return bestScore;
        }
        else // 상대방(최소화 플레이어) 차례
        {
            var bestScore = float.MaxValue;
            
            // 위 조건문으로 제한한 영역만 탐색
            for (var row = startRow; row <= endRow; row++)
            {
                for (var col = startCol; col <= endCol; col++)
                {
                    if (board[row, col] == Constants.PlayerType.None)
                    {
                        board[row, col] = Constants.PlayerType.PlayerA;
                        var score = DoMiniMax(board, depth + 1, true, alpha, beta);
                        board[row, col] = Constants.PlayerType.None; 

                        bestScore = Mathf.Min(score, bestScore);
                        beta = Mathf.Min(beta, bestScore);

                        if (alpha >= beta) // 알파-베타 가지치기
                        {
                            break; // 안쪽 for 루프 탈출
                        }
                    }
                }
                if (alpha >= beta) // 알파-베타 가지치기
                {
                    break; // 바깥쪽 for 루프도 탈출
                }
            }
            return bestScore;
        }
    }
    
    /// <summary>
    /// 작성자 : 김동건
    /// 위치에 돌을 놓았을때 비겼는지 확인하는 함수
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public static bool CheckGameDraw(Constants.PlayerType[,] board)
    {
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == Constants.PlayerType.None) return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 작성자 : 김동건
    /// 위치에 돌을 놓았을때 턴 플레이어가 이겼는지 확인하는 함수
    /// </summary>
    /// <param name="playerType"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    public static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    {
        // Col 체크 후 일자면 True
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col < board.GetLength(1) - 4; col++)
            {
                if (board[row, col] == playerType &&
                    board[row, col + 1] == playerType &&
                    board[row, col + 2] == playerType &&
                    board[row, col + 3] == playerType &&
                    board[row, col + 4] == playerType)

                {
                    return true;
                }
            }

        }

        // Row 체크 후 일자면 True
        for (var col = 0; col < board.GetLength(1); col++)
        {
            for (var row = 0; row < board.GetLength(0) - 4; row++)
            {
                if (board[row, col] == playerType &&
                    board[row + 1, col] == playerType &&
                    board[row + 2, col] == playerType &&
                    board[row + 3, col] == playerType &&
                    board[row + 4, col] == playerType)
                {
                    return true;
                }
            }

        }

        // 대각선(좌상단 -> 우하단) 일자면 True
        for (var row = 0; row < board.GetLength(0) - 4; row++)
        {
            for (var col = 0; col < board.GetLength(1) - 4; col++)
            {
                if (board[row, col] == playerType &&
                    board[row + 1, col + 1] == playerType &&
                    board[row + 2, col + 2] == playerType &&
                    board[row + 3, col + 3] == playerType &&
                    board[row + 4, col + 4] == playerType)
                {
                    return true;
                }
            }
        }
        
        // 대각선(우상단 -> 좌하단) 일자면 True
        for (var row = 0; row < board.GetLength(0) - 4; row++)
        {
            for (var col = 4; col < board.GetLength(1); col++)
            {
                if (board[row, col] == playerType &&
                    board[row + 1, col - 1] == playerType &&
                    board[row + 2, col - 2] == playerType &&
                    board[row + 3, col - 3] == playerType &&
                    board[row + 4, col - 4] == playerType)
                {
                    return true;
                }
            }
        }

        return false;
    }
}