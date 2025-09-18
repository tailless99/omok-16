using System;
using System.Collections.Generic;
using Gomoku;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic
{
    public BlockController BlockController;     // Block을 처리할 객체

    private Constants.PlayerType[,] _board;     // 보드의 상태 정보
    private bool[,] _forbiddenMarkersBoard = new bool[15, 15]; // 금지마크 설정을 위한 보드

    public BasePlayerState firstPlayerState;    // Player A
    public BasePlayerState secondPlayerState;   // Player B
    public BasePlayerState _currentPlayerState; // 현재 턴의 Player

    private Constants.GameType _gameType;
    private Constants.GameType GameType => _gameType;

    private int currentTurnCount;

    private List<TurnState> turnHistory = new List<TurnState>();


    public enum GameResult { None, PlayerAWin, PlayerBWin, Draw }


    public GameLogic(BlockController blockController, Constants.GameType gameType) {
        BlockController = blockController;
        _gameType = gameType;

        // 보드의 상태 정보 초기화
        _board = new Constants.PlayerType[Constants.BlockColumnCount, Constants.BlockColumnCount];
        _forbiddenMarkersBoard = new bool[Constants.BlockColumnCount, Constants.BlockColumnCount];

        StartGame();
    }
    
    private void StartGame()
    {
        turnHistory = new List<TurnState>();
        GameManager.Instance.SetupReplayButtons(false);
        BoardReset();
        switch (_gameType)
        {
            case Constants.GameType.SinglePlay:
                firstPlayerState = new PlayerState(true);
                GameManager.Instance.SetPlayerRateTierPanel(GameUIController.GameTurnPanelType.ATurn, firstPlayerState.rateTier, firstPlayerState.currentEXP);
                secondPlayerState = new AIState();
                break;
            case Constants.GameType.DualPlay:
                firstPlayerState = new PlayerState(true);
                GameManager.Instance.SetPlayerRateTierPanel(GameUIController.GameTurnPanelType.ATurn, firstPlayerState.rateTier, firstPlayerState.currentEXP);
                secondPlayerState = new PlayerState(false);
                break;
            case Constants.GameType.MultiPlay:
                break;
        }

        // 게임 시작
        SetState(firstPlayerState);
    }


    // 외부에서 보드를 가져올 수 있도록 반환
    public Constants.PlayerType[,] GetBoard() {
        return _board;
    }
    
    // 외부에서 금수 보드를 가져올 수 있도록 반환
    public bool[,] GetForbiddenMarkersBoard()
    {
        return _forbiddenMarkersBoard;
    }


    // 턴이 바뀔 때, 기존 진행하던 상태를 Exit하고
    // 이번 턴의 상태를 _currentPlayerState로 변경
    public void SetState(BasePlayerState state) {
        _currentPlayerState?.OnExit(this);
        _currentPlayerState = state;

        // 흑돌 턴일 때 금지마크 표시
        UpdateForbiddenMarkersForCurrentPlayer();
        
        _currentPlayerState?.OnEnter(this);
    }

    // _board 배열에 새로운 Marker 값을 할당
    public bool SetNewBoardValue(Constants.PlayerType playerType, int row, int col) {
        if (_board[row, col] != Constants.PlayerType.None) return false;
        
        if (playerType == Constants.PlayerType.PlayerA)
        {
            var intBoard = GetIntBoard();
            if (RenjuRule.IsForbiddenMove(intBoard, row, col, 1) == false)
            {
                _board[row, col] = playerType;
                BlockController.PlaceMarker(Block.MarkerType.blackMarker, row, col);
                currentTurnCount++;
                SaveCurrentTurn(row, col, 1);
                GameManager.Instance.UpdateTurnUI(currentTurnCount, currentTurnCount);
                return true;
            }
        }
        else if(playerType == Constants.PlayerType.PlayerB) {
            _board[row, col] = playerType;
            BlockController.PlaceMarker(Block.MarkerType.whiteMarker, row, col);
            currentTurnCount++;
            SaveCurrentTurn(row, col, 2);
            GameManager.Instance.UpdateTurnUI(currentTurnCount, currentTurnCount);
            return true;
        }

        return false;
    }

    private void UpdateForbiddenMarkersForCurrentPlayer()
    {
        var intBoard = GetIntBoard();
        int currentPlayerId = 0;
        if (_currentPlayerState == firstPlayerState)
        {
            currentPlayerId = 1; // 흑돌
        }
        else if (_currentPlayerState == secondPlayerState)
        {
            currentPlayerId = 2; // 백돌
        }

        // 먼저 이전에 표시됐을지 모르는 금수 마커 초기화
        for (int r = 0; r < Constants.BlockColumnCount; r++)
        {
            for (int c = 0; c < Constants.BlockColumnCount; c++)
            {
                // _forbiddenMarkersBoard[r,c]가 true였다면, 즉 이전에 금수였다면
                // 해당 위치의 마커를 빈칸으로 되돌림
                if (_forbiddenMarkersBoard[r, c])
                {
                    BlockController.PlaceMarker(Block.MarkerType.None, r, c);
                }
            }
        }

        // 게임이 끝났거나(state == null) 플레이어가 정해지지 않은 경우, 금수판을 비우고 종료
        if (_currentPlayerState == null)
        {
            Array.Clear(_forbiddenMarkersBoard, 0, _forbiddenMarkersBoard.Length);
            return;
        }

        // RenjuRule을 사용하여 금수 위치를 새로 계산
        RenjuRule.UpdateForbiddenMarkers(intBoard, currentPlayerId, _forbiddenMarkersBoard);

        // 새로 계산된 금수 위치에 마커를 표시
        for (int r = 0; r < Constants.BlockColumnCount; r++)
        {
            for (int c = 0; c < Constants.BlockColumnCount; c++)
            {
                if (_forbiddenMarkersBoard[r, c])
                {
                    // BlockController를 통해 'forbiddenMarker'를 실제로 화면에 표시
                    BlockController.PlaceMarker(Block.MarkerType.forbiddenMarker, r, c);
                }
            }
        }
    }

    // 보드의 정보를 IsForbiddenMove에 쓰기위해 타입을 변환하여 가져옴
    private int[,] GetIntBoard()
    {
        var intBoard = new int[Constants.BlockColumnCount, Constants.BlockColumnCount];
        for (int r = 0; r < Constants.BlockColumnCount; r++)
        {
            for (int c = 0; c < Constants.BlockColumnCount; c++)
            {
                switch (_board[r, c])
                {
                    case Constants.PlayerType.PlayerA: // 흑돌
                        intBoard[r, c] = 1;
                        break;
                    case Constants.PlayerType.PlayerB: // 백돌
                        intBoard[r, c] = 2;
                        break;
                    default:
                        intBoard[r, c] = 0;
                        break;
                }
            }
        }
        return intBoard;
    }

    public void BoardReset()
    {
        for (int r = 0; r < Constants.BlockColumnCount; r++)
        {
            for (int c = 0; c < Constants.BlockColumnCount; c++)
            {
                _board[r, c] = Constants.PlayerType.None;
                BlockController.PlaceMarker(Block.MarkerType.None, r, c);
            }
        }
        Array.Clear(_forbiddenMarkersBoard, 0, _forbiddenMarkersBoard.Length);
    }
    
    // Game Over 처리
    public void EndGame(GameResult gameResult)
    {
        GameManager.Instance.GetTurnData();
        GameManager.Instance.SetupReplayButtons(true);
        SetState(null);
        firstPlayerState = null;
        secondPlayerState = null;

        var streak = GameObject.FindAnyObjectByType<WinningStreak>();
        if (streak != null)
        {
            streak.WinningCount(gameResult);
        }

        // 유저에게 Game Over 표시
        GameManager.Instance.OpenConfirmPanel("게임 오버", () => {
            GameManager.Instance.OpenRematchPanel("재도전 하시겠습니까?", () =>
            {
                StartGame();
            });
        });
    }

    // 게임의 결과 확인
    public GameResult CheckGameResult(int row, int col)
    {
        if (OmokAI.CheckGameWin(Constants.PlayerType.PlayerA, _board, row, col))
        {
            return GameResult.PlayerAWin; // 플레이어 A 승리 체크
        }

        if (OmokAI.CheckGameWin(Constants.PlayerType.PlayerB, _board, row, col))
        {
            return GameResult.PlayerBWin; // 플레이어 B 승리 체크
        }

        if (OmokAI.CheckGameDraw(_board)) return GameResult.Draw; // 비겼는지 확인

        // 다 아니라면, 아직 승부중이므로 None 상태 반환
        return GameResult.None;
    }
    
    // 기보 시스템
    private void SaveCurrentTurn(int row, int col, int player)
    {
        var gameBoard = GetIntBoard();
        TurnState turnState = new TurnState(gameBoard, currentTurnCount, player, row, col);
        turnHistory.Add(turnState);
    }

    public List<TurnState> GetTurnHistory()
    {
        return turnHistory;
    }
}
