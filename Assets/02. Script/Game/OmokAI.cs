using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Task 사용을 위해 추가

public static class OmokAI
{
    private static int searchPadding = 1; // 주변 몇 칸까지 탐색할지 설정
    
    /// <summary>
    /// 작성자 : 김동건
    /// 최적의 수를 판단하기 위한 후보 좌표를 List로 저장하는 함수
    /// </summary>
    /// <param name="board"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    private static List<(int row, int col)> GetSearchCandidates(Constants.PlayerType[,] board, int padding)
    {
        var candidates = new List<(int row, int col)>();
        int boardRows = board.GetLength(0);
        int boardCols = board.GetLength(1);
        bool hasStone = false;

        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardCols; j++)
            {
                if (board[i, j] != Constants.PlayerType.None)
                {
                    hasStone = true;
                    
                    for (int dr = -padding; dr <= padding; dr++) // searchPadding 영역 중 돌이 없는 자리 탐색
                    {
                        for (int dc = -padding; dc <= padding; dc++)
                        {
                            int newRow = i + dr;
                            int newCol = j + dc;

                            if (newRow >= 0 && newRow < boardRows &&
                                newCol >= 0 && newCol < boardCols &&
                                board[newRow, newCol] == Constants.PlayerType.None)
                            {
                                candidates.Add((newRow, newCol)); // 찾았다면 후보 리스트에 추가
                            }
                        }
                    }
                }
            }
        }

        if (!hasStone && candidates.Count == 0) // 만약 현재 놓여진 돌이 없고 후보가 없다면 보드 중앙을 후보로 추가
        {
            candidates.Add((boardRows / 2, boardCols / 2));
        }
        
        return candidates.Distinct().ToList(); // 중복 제거 후 리스트 반환
    }

    /// <summary>
    /// 작성자 : 김동건
    /// 현재 보드 상태를 전달하면 다음 최적의 좌표를 반환하는 함수
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    public static async Task<(int row, int col)?> GetBestMove(Constants.PlayerType[,] board)
    {
        float bestScore = float.MinValue;
        (int row, int col) movePosition = (-1, -1);
        
        // 최적의 수일 가능성이 높은 후보 리스트
        var candidateMoves = GetSearchCandidates(board, searchPadding);

        // 비동기 함수들을 관리할 리스트 변수
        var tasks = new List<Task<(float score, int row, int col)>>();
        
        // 비동기 함수가 끝나고 업데이트를 순서대로 하기 위한 Lock 오브젝트
        object lockObject = new object();

        // 각 후보에 대해 Task를 생성하고 시작
        foreach (var move in candidateMoves)
        {
            int row = move.row;
            int col = move.col;

            if (board[row, col] == Constants.PlayerType.None)
            {
                // 각 Task가 독립적으로 작업 되기때문에 보드 또한 복사하여 사용해야 함
                Constants.PlayerType[,] boardCopy = new Constants.PlayerType[board.GetLength(0), board.GetLength(1)];
                for (int r = 0; r < board.GetLength(0); r++)
                {
                    for (int c = 0; c < board.GetLength(1); c++)
                    {
                        boardCopy[r, c] = board[r, c];
                    }
                }

                boardCopy[row, col] = Constants.PlayerType.PlayerB;
                
                // 비동기로 DoMiniMax 함수 호출
                tasks.Add(Task.Run(() => 
                {
                    float score = DoMiniMax(boardCopy, 0, false, float.MinValue, float.MaxValue, row, col);
                    
                    return (score, row, col);
                }));
            }
        }
        
        // await 때문에 모든 Task가 완료 될때 까지 대기
        var results = await Task.WhenAll(tasks);

        // 결과를 취합하여 최적의 수 결정
        foreach (var result in results)
        {
            // 여러 Task가 동시에 bestScore를 업데이트하지 않도록 Lock 사용
            lock (lockObject)
            {
                if (result.score > bestScore)
                {
                    bestScore = result.score;
                    movePosition = (result.row, result.col);
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
    /// <param name="lastPlacedRow"></param>
    /// <param name="lastPlacedCol"></param>
    /// <returns></returns>
    private static float DoMiniMax(Constants.PlayerType[,] board, int depth, bool isMaximizing, float alpha, float beta, int lastPlacedRow, int lastPlacedCol)
    {
        // 게임 종료 상태 체크 (isMaximizing 값에 따라 차례 변경)
        Constants.PlayerType lastPlayer = isMaximizing ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB; // 방금 돌을 놓은 플레이어
        
        if (CheckGameWin(lastPlayer, board, lastPlacedRow, lastPlacedCol)) // 오목이 만들어 지는 경우 차례에 따른 점수 설정
        {
            return (lastPlayer == Constants.PlayerType.PlayerB) ? (2250000 - depth * 100) : (-2250000 + depth * 100); 
        }
        
        if (CheckGameDraw(board) || depth >= 3 ) // 비기거나 최대 깊이 도달시 EvaluateBoard(평가 함수)를 사용하여 가중치 계산
        {
            return EvaluateBoard(board, Constants.PlayerType.PlayerB) - EvaluateBoard(board, Constants.PlayerType.PlayerA);
        }

        if (isMaximizing) // AI(최대화 플레이어) 차례
        {
            var bestScore = float.MinValue;
            var candidateMoves = GetSearchCandidates(board, searchPadding);

            foreach (var move in candidateMoves)
            {
                int row = move.row;
                int col = move.col;

                if (board[row, col] == Constants.PlayerType.None)
                {
                    board[row, col] = Constants.PlayerType.PlayerB;
                    var score = DoMiniMax(board, depth + 1, false, alpha, beta, row, col);
                    board[row, col] = Constants.PlayerType.None;

                    bestScore = Mathf.Max(score, bestScore);
                    alpha = Mathf.Max(alpha, bestScore);

                    if (alpha >= beta) break;
                }
            }
            return bestScore;
        }
        else // 상대방(최소화 플레이어) 차례
        {
            var bestScore = float.MaxValue;
            var candidateMoves = GetSearchCandidates(board, searchPadding);

            foreach (var move in candidateMoves)
            {
                int row = move.row;
                int col = move.col;

                if (board[row, col] == Constants.PlayerType.None)
                {
                    board[row, col] = Constants.PlayerType.PlayerA;
                    var score = DoMiniMax(board, depth + 1, true, alpha, beta, row, col);
                    board[row, col] = Constants.PlayerType.None;

                    bestScore = Mathf.Min(score, bestScore);
                    beta = Mathf.Min(beta, bestScore);

                    if (alpha >= beta) break;
                }
            }
            return bestScore;
        }
    }
    
    /// <summary>
    /// 작성자 : 김동건
    /// 연속된 돌의 개수를 파악 및 양쪽 끝이 열려있는지 닫혀있는지 확인하는 함수
    /// </summary>
    /// <param name="board"></param>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="dr"></param>
    /// <param name="dc"></param>
    /// <param name="playerType"></param>
    /// <returns></returns>
    private static (int count, int openEnds, int closedEnds, int adjacentOwnStones, int adjacentOpponentStones) CheckLine(
        Constants.PlayerType[,] board, int r, int c, int dr, int dc, Constants.PlayerType playerType)
    {
        int boardRows = board.GetLength(0);
        int boardCols = board.GetLength(1);
        Constants.PlayerType opponentType = (playerType == Constants.PlayerType.PlayerA) ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;

        int count = 0;
        int openEnds = 0;
        int closedEnds = 0;
        int adjacentOwnStones = 0;
        int adjacentOpponentStones = 0; // 가중치 조건에 맞게 '상대방 돌 인접' 판단을 위해 추가

        int currentR = r;
        int currentC = c;

        // 돌의 시작점 찾기: 주어진 (r, c)가 연속된 돌의 중간일 수 있으므로 시작점까지 거슬러 올라감
        while (currentR - dr >= 0 && currentR - dr < boardRows &&
               currentC - dc >= 0 && currentC - dc < boardCols &&
               board[currentR - dr, currentC - dc] == playerType)
        {
            currentR -= dr;
            currentC -= dc;
        }

        // 연속된 돌 세기
        int tempR = currentR;
        int tempC = currentC;
        while (tempR >= 0 && tempR < boardRows &&
               tempC >= 0 && tempC < boardCols &&
               board[tempR, tempC] == playerType)
        {
            count++;
            tempR += dr;
            tempC += dc;
        }

        // 양 끝 상태 확인
        // 첫 번째 끝
        int end1R = currentR - dr;
        int end1C = currentC - dc;
        if (end1R >= 0 && end1R < boardRows && end1C >= 0 && end1C < boardCols)
        {
            if (board[end1R, end1C] == Constants.PlayerType.None)
            {
                openEnds++;
            }
            else if (board[end1R, end1C] == opponentType)
            {
                closedEnds++;
                adjacentOpponentStones++; // 적돌 인접
            }
            else if (board[end1R, end1C] == playerType) // 자신의 돌이 인접
            {
                adjacentOwnStones++;
            }
        }
        else // 보드 경계
        {
            closedEnds++;
        }

        // 두 번째 끝
        int end2R = tempR; // 연속된 돌이 끝난 다음 칸
        int end2C = tempC;
        if (end2R >= 0 && end2R < boardRows && end2C >= 0 && end2C < boardCols)
        {
            if (board[end2R, end2C] == Constants.PlayerType.None)
            {
                openEnds++;
            }
            else if (board[end2R, end2C] == opponentType)
            {
                closedEnds++;
                adjacentOpponentStones++; // 적돌 인접
            }
            else if (board[end2R, end2C] == playerType) // 자신의 돌이 인접
            {
                adjacentOwnStones++;
            }
        }
        else // 보드 경계
        {
            closedEnds++;
        }
        
        return (count, openEnds, closedEnds, adjacentOwnStones, adjacentOpponentStones);
    }
    
    /// <summary>
    /// 작성자 : 김동건
    /// 현재 보드 상태를 평가하여 가중치에 따른 점수를 반환하는 함수
    /// 가중치 참고 : https://github.com/WONYOUNG-HC/AI-Gomoku?tab=readme-ov-file#alpha-beta-pruning
    /// </summary>
    /// <param name="board"></param>
    /// <param name="turnPlayer"></param>
    /// <returns></returns>
    private static float EvaluateBoard(Constants.PlayerType[,] board, Constants.PlayerType turnPlayer)
    {
        float totalScore = 0;
        int boardRows = board.GetLength(0);
        int boardCols = board.GetLength(1);
        
        Constants.PlayerType opponentType = (turnPlayer == Constants.PlayerType.PlayerA) ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;

        // 가로, 세로, 두 대각선 방향 벡터
        int[,] directions = new int[,]
        {
            {0, 1},   // 가로 (오른쪽)
            {1, 0},   // 세로 (아래)
            {1, 1},   // 대각선 (우하향)
            {1, -1}   // 대각선 (좌하향)
        };

        // 방문한 돌의 시작 위치를 저장하여 중복 계산을 피함 (예: 가로 5개가 있을 때, 첫 번째 돌에서 한 번만 평가)
        HashSet<(int r, int c, int dr, int dc)> visitedStartingPoints = new HashSet<(int r, int c, int dr, int dc)>();

        for (int r = 0; r < boardRows; r++)
        {
            for (int c = 0; c < boardCols; c++)
            {
                if (board[r, c] == turnPlayer) // 평가하려는 플레이어의 돌인 경우
                {
                    for (int i = 0; i < directions.GetLength(0); i++)
                    {
                        int dr = directions[i, 0];
                        int dc = directions[i, 1];

                        // 이 위치와 방향에서 이어진 선이 이미 시작점으로 방문되었는지 확인
                        int currentR = r;
                        int currentC = c;
                        // 돌의 시작점 찾기 (현재 위치가 중간일 수 있으므로)
                        while (currentR - dr >= 0 && currentR - dr < boardRows &&
                               currentC - dc >= 0 && currentC - dc < boardCols &&
                               board[currentR - dr, currentC - dc] == turnPlayer)
                        {
                            currentR -= dr;
                            currentC -= dc;
                        }
                        if (visitedStartingPoints.Contains((currentR, currentC, dr, dc)))
                        {
                            continue; // 이미 계산된 라인
                        }
                        visitedStartingPoints.Add((currentR, currentC, dr, dc));


                        var (count, openEnds, closedEnds, adjacentOwnStones, adjacentOpponentStones) = 
                            CheckLine(board, currentR, currentC, dr, dc, turnPlayer);
                        
                        if (count == 4) // --- 4목일 때 가중치 ---
                        {
                             // 5목이 가능한 위치 (오픈된 한쪽 끝이 있어야 함)
                            if (openEnds >= 1) 
                            {
                                totalScore += 5000;
                            }
                            // 닫힌 4목 (더 이상 5목이 될 수 없는 경우)
                            else if (closedEnds == 2)
                            {
                                // 사실상 의미 없는 라인이지만, 필요하다면 낮은 점수 부여
                                totalScore += 10;
                            }
                        }
                        else if (count == 3)    // --- 3목일 때 가중치 ---
                        {
                            if (openEnds == 2 && adjacentOpponentStones == 0) // 양쪽 열려있음 + 적돌 인접 X
                            {
                                totalScore += 500;
                            }
                            else if (openEnds == 1 && adjacentOpponentStones == 0) // 한쪽 열려있음 + 적돌 인접 X
                            {
                                totalScore += 200; 
                            }
                            else if (openEnds >= 1 && adjacentOpponentStones >= 1) // 적돌 인접 O
                            {
                                totalScore += 57;
                            }
                            else if (openEnds == 0 && closedEnds == 2) // 양방향 막힘 (오목 불가)
                            {
                                totalScore += 57;
                            }
                        }
                        else if (count == 2)    // --- 2목일 때 가중치 ---
                        {
                             if (openEnds == 2 && adjacentOpponentStones == 0) // 양쪽 열려있음 + 적돌 인접 X
                             {
                                 totalScore += 50;
                             }
                             else if (openEnds == 1 && adjacentOpponentStones == 0) // 한쪽 열려있음 + 적돌 인접 X
                             {
                                 totalScore += 25; // 55점 조건과 유사하게 조정
                             }
                             else if (openEnds >= 1 && adjacentOpponentStones >= 1) // 적돌 인접 O
                             {
                                 totalScore += 30;
                             }
                             else if (openEnds == 0 && closedEnds == 2) // 양방향 막힘
                             {
                                 totalScore += 30;
                             }
                        }
                        else if (count == 1)    // --- 1목일 때 가중치 ---
                        {
                            if (openEnds == 2 && adjacentOpponentStones == 0) // 양쪽 열려있음 + 적돌 인접 X
                            {
                                totalScore += 10; // "적돌과 인접하지 않음 + 자신의 돌과 인접하지 않음"
                            }
                            else if (openEnds >= 1 && adjacentOpponentStones == 0) // 한쪽 열려있음 + 적돌 인접 X
                            {
                                totalScore += 7; // "적돌과 인접하지 않음 + 자신의 돌과 인접"이 되려면 이 돌이 다른 돌과 인접해야 하는데, 그 조건은 현재 미고려
                            }
                            else if (adjacentOpponentStones >= 1) // 적돌 인접 O
                            {
                                totalScore += 5; // "적돌과 인접 + 자신의 돌과 인접하지 않음" 혹은 "적돌과 인접 + 자신의 돌과 인접" (단독으로 인접할 때)
                            }
                            else if (openEnds == 0 && closedEnds == 2) // 양방향 막힘
                            {
                                totalScore += 5; // "오목이 되는 길이 유일할때" 조건과 유사하게
                            }
                        }
                    }
                }
            }
        }
        return totalScore;
    }

    
    /// <summary>
    /// 작성자 : 김동건
    /// 무승부를 확인하기 위해서 보드에 자리가 있는지 확인하는 함수
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
    /// 마지막 착수 위치를 받아 해당 위치를 중심으로 승리 조건을 확인하는 함수
    /// </summary>
    /// <param name="playerType"></param>
    /// <param name="board"></param>
    /// <param name="lastPlacedRow"></param>
    /// <param name="lastPlacedCol"></param>
    /// <returns></returns>
    public static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board, int lastPlacedRow, int lastPlacedCol)
    {
        int boardRows = board.GetLength(0);
        int boardCols = board.GetLength(1);

        // 검사할 방향 벡터들: (dr, dc)
        int[,] directions = new int[,]
        {
            {0, 1},   // 가로 (오른쪽)
            {1, 0},   // 세로 (아래)
            {1, 1},   // 대각선 (우하향)
            {1, -1}   // 대각선 (좌하향)
        };
        
        // 오목을 검사하기 위해 입력된 돌의 좌표를 기준으로 4방향에 대해 검사
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dr = directions[i, 0];
            int dc = directions[i, 1];

            // 이 방향과 반대 방향을 모두 고려하여 5목 라인이 있는지 확인
            // 현재 놓인 돌을 기준으로 시작점 (5개 돌 중 첫 번째 돌)을 찾기 위해 최대 4칸 뒤로 이동하며 검사
            for (int k = 0; k < 5; k++)
            {
                int startRow = lastPlacedRow - dr * k;
                int startCol = lastPlacedCol - dc * k;

                int count = 0;
                
                // 시작점(startRow, startCol)에서부터 5칸을 이어서 검사
                for (int step = 0; step < 5; step++)
                {
                    int r = startRow + dr * step;
                    int c = startCol + dc * step;

                    // 보드 범위 내에 있고, 현재 플레이어의 돌이면 count 증가
                    if (r >= 0 && r < boardRows && c >= 0 && c < boardCols && board[r, c] == playerType)
                    {
                        count++;
                    }
                    else // 중간에 다른 돌이나 빈 칸이 있으면 이 라인은 5목이 아님
                    {
                        count = 0; // 초기화
                        break;
                    }
                }

                if (count == 5)
                {
                    return true;
                }
            }
        }
        return false;
    }
}