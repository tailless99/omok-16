using UnityEngine;
using static Constants;

public abstract class BasePlayerState {
    public bool _isFirstPlayer;

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
        if (gameLogic.SetNewBoardValue(playerType, row, col)) {
            // 새롭게 놓여진 Marker를 기반으로 게임의 결과를 판단
            gameResult = gameLogic.CheckGameResult(row, col);

            if (gameResult == GameLogic.GameResult.None) {
                HandleNextTurn(gameLogic);
            }
            else {
                // 변경 전 경험치 및 골드 저장
                var prevExp = currentEXP;
                var prevGold = GameManager.Instance.haveGold;
                var _rankType = Constants.RankChangeType.None; // 랭크업 여부

                // 경험치 및 골드 업데이트
                UpdatePlayerRate(gameLogic, gameResult, playerType, _isFirstPlayer, out _rankType);

                // 변경 후 경험치 및 골드 저장
                var nextExp = currentEXP;
                var nextGold = GameManager.Instance.haveGold;

                // 게임 결과에 따라 경험치 및 골드 획득 안내 패널 업데이트
                GameUIController.onRewardPanelUpdate?.Invoke(prevExp, nextExp, prevGold, nextGold, _rankType);

                // TODO : gameLogic에게 Game Over 전달
                gameLogic.EndGame(gameResult);
            }
        }
    }

    /// <summary>
    /// 작성자 : 이동현
    /// 게임의 결과에 따라 경험치와 골드 변화를 처리
    /// </summary>
    private void UpdatePlayerRate(GameLogic gameLogic, GameLogic.GameResult gameResult, Constants.PlayerType playerType, bool isFirstPlayer,
        out Constants.RankChangeType rankType) {
        // 획득 골드와 경험치 변화량 초기화
        var gieGold = 0;
        var expChange = 0;

        // 현재 함수를 호출하는 플레이어가 누구인지에 따라 localPlayer를 할당
        BasePlayerState localPlayer;

        if (isFirstPlayer) {
            localPlayer = gameLogic.firstPlayerState;
        }
        else {
            localPlayer = gameLogic.secondPlayerState;
        }
        
        // 게임 결과에 따라 경험치와 골드 계산
        // 로컬 플레이어가 승리했는지 판단
        bool isLocalPlayerWin = false;
        if (GameManager.Instance.localPlayer == Constants.PlayerType.PlayerA && gameResult == GameLogic.GameResult.PlayerAWin) {
            isLocalPlayerWin = true;
        }
        else if (GameManager.Instance.localPlayer == Constants.PlayerType.PlayerB && gameResult == GameLogic.GameResult.PlayerBWin) {
            isLocalPlayerWin = true;
        }

        if (isLocalPlayerWin) {
            // 승리 시 경험치 증가 (부스터 적용) 및 승리 골드 지급
            var expBooster = GameManager.Instance.isExpIncreaseActive ? 2 : 1;
            expChange = 1 * expBooster;
            gieGold = Constants.winGold;
        }
        else {
            // 패배 시 경험치 감소 (아이템 적용) 및 패배 골드 지급
            if (GameManager.Instance.isExpDecreaseActive) {
                GameManager.Instance.isExpDecreaseActive = false; // 경험치 감소 아이템 사용 후 비활성화
            }
            else {
                expChange = -1;
            }
            gieGold = Constants.loseGold;
        }

        // 로컬 플레이어의 경험치 업데이트
        localPlayer.currentEXP += expChange;

        // 티어 랭크 업 판정
        var requireExp = localPlayer.rateTier >= 10 ?
            Constants.minTierExp : localPlayer.rateTier >= 5 ?
            Constants.middleTierExp : Constants.maxTierExp;

        // 랭크업 조건
        rankType = Constants.RankChangeType.None; // 초기값 설정
        if (localPlayer.currentEXP >= requireExp) {
            // 최고 랭크일 때는 더 이상 랭크업하지 않음
            if (localPlayer.rateTier > Constants.minTier) {
                localPlayer.rateTier--;
                localPlayer.currentEXP -= requireExp;

                rankType = Constants.RankChangeType.RankUp; // 랭크 업 반환
            }
        }
        // 랭크다운 조건
        else if (localPlayer.currentEXP < 0) {
            // 최하위 랭크일 때는 더 이상 랭크다운하지 않음
            if (localPlayer.rateTier < Constants.maxTier) {
                localPlayer.rateTier++;
                localPlayer.currentEXP += requireExp;

                rankType = Constants.RankChangeType.RankDown; // 랭크 다운 반환
            }
        }

        // 최종적으로 로컬 플레이어의 티어 및 경험치, 골드 정보 업데이트
        GameManager.Instance.SetTierInfo(localPlayer.rateTier, localPlayer.currentEXP, gieGold);
    }
}
