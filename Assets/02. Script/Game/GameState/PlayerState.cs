using UnityEngine;

public class PlayerState : BasePlayerState {
    private bool _isFirstPlayer;
    private Constants.PlayerType _playerType;


    // 생성자 초기화
    public PlayerState(bool isFirstPlayer) {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ? Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;

        // TODO : 멀티 서버 연결 후, 플레이어의 티어 정보 업데이트
        GameManager.Instance.GetTierInfo(out rateTier, out currentEXP); // 임시변수
    }

    #region 필수 메서드
    public override void OnEnter(GameLogic gameLogic) {
        // 1. First Player인지 확인해서 게임 UI에 현재 턴 표시
        if (_isFirstPlayer) {
            GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.ATurn);
        }
        else {
            GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.BTurn);
        }

        // 2. Block Controller에게 해야 할 일을 전달
        gameLogic.BlockController.OnBlockClickedDelegate = (row, col) => {
            // Block이 터치 될 때까지 기다렸다가
            // 터치 되면 처리할 일
            HandleMove(gameLogic, row, col);
        };
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col) {
        ProcessMove(gameLogic, _playerType, row, col);

        // 게임 종료 시, 급수 경험치 증감
        if (gameResult != GameLogic.GameResult.None)
            UpdatePlayerRate(gameResult);
    }

    protected override void HandleNextTurn(GameLogic gameLogic) {
        if (_isFirstPlayer) {
            gameLogic.SetState(gameLogic.secondPlayerState);
        }
        else {
            gameLogic.SetState(gameLogic.firstPlayerState);
        }
    }

    public override void OnExit(GameLogic gameLogic) {
        gameLogic.BlockController.OnBlockClickedDelegate = null;
    }

    // 작성자 : 이동현
    // 게임 결과에 따라 플레이어의 급수 경험치 업데이트
    private void UpdatePlayerRate(GameLogic.GameResult gameResult) {
        var gieGold = 0; // 획득 골드
        var expBosster = GameManager.Instance.isExpIncreaseActive ? 2 : 1; // 경험치 추가 증가

        // Player A의 경우
        if (_playerType == Constants.PlayerType.PlayerA) {
            if (gameResult == GameLogic.GameResult.PlayerAWin) {
                currentEXP += 1 * expBosster; // 경험치 1 증가
                gieGold = Constants.winGold; // 승리 골드 지급
            }
            else if (gameResult == GameLogic.GameResult.PlayerBWin) {
                // 경험치 감소 아이템이 있을 때
                if (GameManager.Instance.isExpDecreaseActive) {
                    currentEXP -= 0;
                    GameManager.Instance.isExpDecreaseActive = false; // 경험치 감소 아이템 사용 후 비활성화
                }
                // 경험치 감소 아이템이 없을 때
                else {
                    currentEXP -= 1; // 경험치 1 감소
                }

                gieGold = Constants.loseGold; // 패배 골드 지급
            }
        }

        // Player B의 경우
        if (_playerType == Constants.PlayerType.PlayerB) {
            if (gameResult == GameLogic.GameResult.PlayerAWin) {
                // 경험치 감소 아이템이 있을 때
                if (GameManager.Instance.isExpDecreaseActive) {
                    currentEXP -= 0;
                    GameManager.Instance.isExpDecreaseActive = false; // 경험치 감소 아이템 사용 후 비활성화
                }
                // 경험치 감소 아이템이 없을 때
                else {
                    currentEXP -= 1; // 경험치 1 감소
                }
                gieGold = Constants.loseGold; // 패배 골드 지급
            }
            else if (gameResult == GameLogic.GameResult.PlayerBWin) {
                currentEXP += 1 * expBosster; // 경험치 1 증가
                gieGold = Constants.winGold; // 승리 골드 지급
            }
        }

        // 티어 랭크 업 판정
        // 하위 랭크 : 3, 중간 랭크 : 5, 상위 랭크 : 10
        var requireExp = rateTier >= 10 ?
            Constants.minTierExp : rateTier >= 5 ?
            Constants.middleTierExp : Constants.maxTierExp; // 필요 경험치

        // 랭크업 조건 만족
        if (currentEXP >= requireExp) {
            // 최고 랭크일 때
            if (rateTier <= maxTier) {
                return;
            }
            else {
                rateTier -= 1; // 티어 1단계 상승
                currentEXP -= requireExp;
            }
        }
        // 랭크다운 조건 만족
        else if (currentEXP < requireExp * -1) {
            // 티어 랭크 다운
            if (rateTier >= minTier) {
                return;
            }
            else {
                rateTier += 1; // 티어 1단계 하락
                currentEXP -= requireExp;
            }
        }
        
        GameManager.Instance.SetPlayerRateTierPanel(GameUIController.GameTurnPanelType.ATurn, rateTier, currentEXP);
        // TODO : 멀티 서버 연결 후, 플레이어의 티어 및 경험치 업데이트
        GameManager.Instance.SetTierInfo(rateTier, currentEXP, gieGold); // 임시변수
    }
    #endregion
}
