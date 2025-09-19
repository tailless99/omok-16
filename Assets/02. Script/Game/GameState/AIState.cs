using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 작성자 : 김동건
/// 오목 AI의 차례 및 착수 까지의 행동을 구현한 스크립트
/// </summary>
public class AIState : BasePlayerState {
    /// <summary>
    /// 작성자 : 김동건
    /// AI의 차례가 되면 현재 보드를 가져와 최적의 수를 연산하는 함수 GetBestMove를 호출함
    /// GetBestMove의 후보군마다 Minimax 알고리즘 연산 시간이 너무 오래 걸리므로 화면이 멈춘 것처럼 보임
    /// 따라서 GetBestMove의 후보군의 DoMiniMax 함수를 비동기 연산을 함으로써 속도 개선 및 연산 중 상호작용 가능
    /// </summary>
    /// <param name="gameLogic"></param>
    public override async void OnEnter(GameLogic gameLogic) {
        // 턴 표시
        GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.BTurn);
        
        // AI 처리
        var board = gameLogic.GetBoard();

        int difficulty = GetDifficulty();
        
        // GetBestMove 연산을 백그라운드 스레드에서 실행, await 때문에 GetBestMove 함수가 완료 될 때까지 대기
        (int row, int col)? result = await OmokAI.GetBestMove(board , difficulty);
        
        
        if (result.HasValue) {
            HandleMove(gameLogic, result.Value.row, result.Value.col);
        }
        else {
            gameLogic.EndGame(GameLogic.GameResult.Draw);
        }
    }
    
    /// <summary>
    /// 작성자 : 김동건
    /// 플레이어의 티어에 따라 최대 연산 횟수 제한으로 AI 난이도 차이 구현
    /// </summary>
    /// <returns></returns>
    private int GetDifficulty()
    {
        int difficulty = 0;
        GameManager.Instance.GetTierInfo(out int tier, out int tierExp);
        
        switch (tier/5)
        {
            case 3: // 급수 15 ~ 18
                difficulty = 1;
                break;
            case 2: // 급수 10 ~ 14
                difficulty = 5;
                break;
            case 1: // 급수 9 ~ 5
                difficulty = 15;
                break;
            case 0: // 급수 1 ~ 4
                difficulty = 100;
                break;
        }

        return difficulty;
    }
    
    protected override void HandleNextTurn(GameLogic gameLogic) {
        gameLogic.SetState(gameLogic.firstPlayerState);
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col) {
        ProcessMove(gameLogic, Constants.PlayerType.PlayerB, row, col);
    }
    
    public override void OnExit(GameLogic gameLogic) {
    }
}