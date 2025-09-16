using System.Reflection;
using UnityEngine;
using System.Threading.Tasks; // Task를 사용하기 위해 필요

/// <summary>
/// 작성자 : 김동건
/// 오목 AI의 차례 및 착수 까지의 행동을 구현한 스크립트
/// </summary>
public class AIState : BasePlayerState {
    /// <summary>
    /// 작성자 : 김동건
    /// AI의 차례가 되면 현재 보드를 가져와 최적의 수를 연산하는 함수 GetBestMove를 호출함
    /// GetBestMove의 연산 시간이 너무 오래 걸리므로 화면이 멈춘 것처럼 보임 따라서 비동기 연산을 함으로써
    /// 설정 창과 상호작용은 물론 게임이 진행되는 것처럼 보임
    /// </summary>
    /// <param name="gameLogic"></param>
    public override async void OnEnter(GameLogic gameLogic) { 
        
        GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.BTurn);

        var board = gameLogic.GetBoard();

        // GetBestMove 연산을 백그라운드 스레드에서 실행
        var result = await Task.Run(() => OmokAI.GetBestMove(board)); 
        // await 덕분에 이 다음 라인은 GetBestMove가 완료될 때까지 기다립니다.

        // GetBestMove 연산 완료 후 다시 메인 스레드로 돌아와 처리
        if (result.HasValue) {
            HandleMove(gameLogic, result.Value.row, result.Value.col);
        }
        else {
            gameLogic.EndGame(GameLogic.GameResult.Draw);
        }
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col) {
        ProcessMove(gameLogic, Constants.PlayerType.PlayerB, row, col);
    }

    protected override void HandleNextTurn(GameLogic gameLogic) {
        gameLogic.SetState(gameLogic.firstPlayerState);
    }

    public override void OnExit(GameLogic gameLogic) {
    }
}