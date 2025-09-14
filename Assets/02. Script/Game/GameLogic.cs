using System;
using Gomoku;
using UnityEngine;

public class GameLogic
{
    public BlockController BlockController;     // Block을 처리할 객체

    private Constants.PlayerType[,] _board;     // 보드의 상태 정보

    public BasePlayerState firstPlayerState;    // Player A
    public BasePlayerState secondPlayerState;   // Player B
    public BasePlayerState _currentPlayerState; // 현재 턴의 Player


    public enum GameResult { None, PlayerAWin, PlayerBWin, Draw }


    public GameLogic(BlockController blockController, Constants.GameType gameType) {
        BlockController = blockController;

        // 보드의 상태 정보 초기화
        _board = new Constants.PlayerType[Constants.BlockColumnCount, Constants.BlockColumnCount];
        // Game Type 초기화
        switch (gameType) {
            case Constants.GameType.SinglePlay:
                firstPlayerState = new PlayerState(true);
                GameManager.Instance.SetPlayerRateTierPanel(GameUIController.GameTurnPanelType.ATurn, firstPlayerState.rateTier, firstPlayerState.currentEXP);
                secondPlayerState = new AIState();
                break;
            case Constants.GameType.DualPlay:
                firstPlayerState = new PlayerState(true);
                GameManager.Instance.SetPlayerRateTierPanel(GameUIController.GameTurnPanelType.ATurn, firstPlayerState.rateTier, firstPlayerState.currentEXP);
                secondPlayerState = new PlayerState(false);
                GameManager.Instance.SetPlayerRateTierPanel(GameUIController.GameTurnPanelType.ATurn, secondPlayerState.rateTier, secondPlayerState.currentEXP);
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

    // 턴이 바뀔 때, 기존 진행하던 상태를 Exit하고
    // 이번 턴의 상태를 _currentPlayerState로 변경
    // 
    public void SetState(BasePlayerState state) {
        _currentPlayerState?.OnExit(this);
        _currentPlayerState = state;
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
            }
            return true;
        }
        else if(playerType == Constants.PlayerType.PlayerB) {
            _board[row, col] = playerType;
            BlockController.PlaceMarker(Block.MarkerType.whiteMarker, row, col);
            return true;
        }

        return false;
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


    // Game Over 처리
    public void EndGame(GameResult gameResult) {
        SetState(null);
        firstPlayerState = null;
        secondPlayerState = null;

        // 유저에게 Game Over 표시
        GameManager.Instance.OpenConfirmPanel("게임 오버", () => {
            GameManager.Instance.ChangeToMainScene();
        });
    }

    // 게임의 결과 확인
    public GameResult CheckGameResult() {
        if(TicTacToeAI.CheckGameWin(Constants.PlayerType.PlayerA, _board)) return GameResult.PlayerAWin; // 플레이어 A 승리 체크
        if(TicTacToeAI.CheckGameWin(Constants.PlayerType.PlayerB, _board)) return GameResult.PlayerBWin; // 플레이어 B 승리 체크
        if(TicTacToeAI.CheckGameDraw(_board)) return GameResult.Draw; // 비겼는지 확인

        // 다 아니라면, 아직 승부중이므로 None 상태 반환
        return GameResult.None;
    }
}
