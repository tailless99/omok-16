using UnityEngine;

public abstract class BasePlayerState
{
    public int rateTier;       // 급수
    public int currentEXP;     // 현재 경험치
    protected int minTier = 18;   // 최하급 티어
    protected int maxTier = 1;    // 최고 티어

    // 게임 결과 저장
    protected GameLogic.GameResult gameResult;

    public abstract void OnEnter(GameLogic gameLogic);      // 상태가 시작
    public abstract void OnExit(GameLogic gameLogic);       // 상태가 종료
    public abstract void HandleMove(GameLogic gameLogic, int row, int col);     // 마커 표시
    protected abstract void HandleNextTurn(GameLogic gameLogic);    // 턴 전환
    
    /// <summary>
    /// 작성자 : 김동건
    /// 좌표를 입력받아 착수 후 게임 결과 처리
    /// </summary>
    /// <param name="gameLogic"></param>
    /// <param name="playerType"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    protected void ProcessMove(GameLogic gameLogic, Constants.PlayerType playerType, int row, int col) {
        if(gameLogic.SetNewBoardValue(playerType, row, col)) {
            // 새롭게 놓여진 Marker를 기반으로 게임의 결과를 판단
            gameResult = gameLogic.CheckGameResult(row , col);

            if(gameResult == GameLogic.GameResult.None) {
                HandleNextTurn(gameLogic);
            }
            else {
                // TODO : gameLogic에게 Game Over 전달
                gameLogic.EndGame(gameResult);
            }
        }
    }
}
