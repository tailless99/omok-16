using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    // 턴 패널
    [SerializeField] private GameObject playerATurnPanel;
    [SerializeField] private GameObject playerBTurnPanel;

    // AI 턴일때 로딩 이미지 출력을 위한 변수
    [SerializeField] private GameObject aiLoadingPanel;
    [SerializeField] private TextMeshProUGUI difficultyText;
    private Constants.GameType _gameType;
    
    // 급수 UI 컨테이너
    [SerializeField] private RateTierPanelController rateTierPanelController;

    // 경험치, 골드 획득 안내 패널
    [SerializeField] private RewardPanelController rewardPanelController;

    /// <summary>
    /// 리워드 패널 업데이트 이벤트
    /// 
    /// </summary>
    public static Action<int, int, int, int, Constants.RankChangeType> onRewardPanelUpdate;

    public enum GameTurnPanelType { None, ATurn, BTurn }

    private void Start()
    {
        GameManager.Instance.GetGameType(out _gameType);
        
        if (_gameType == Constants.GameType.SinglePlay)
        {
            difficultyText.transform.gameObject.SetActive(true);
            GetDifficultyText(difficultyText);
        }
    }

    private void OnEnable() {
        onRewardPanelUpdate += rewardPanelController.InitUI;
    }

    private void OnDisable() {
        onRewardPanelUpdate -= rewardPanelController.InitUI;
    }

    public void OnClickBackButton() {
        // UI 사운드 추가
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        GameManager.Instance.OpenConfirmPanel("게임을 종료하시겠습니까?", () => {
            GameManager.Instance.ChangeToMainScene();
        });
    }

    /// <summary>
    /// 작성자 : 김동건
    /// 난이도에 따른 택스트 출력을 위한 함수
    /// </summary>
    /// <param name="difficultyText"></param>
    private void GetDifficultyText(TextMeshProUGUI difficultyText)
    {
        GameManager.Instance.GetTierInfo(out int tier, out int tierExp);
        
        switch (tier/5)
        {
            case 3: // 급수 15 ~ 18
                difficultyText.text = "<하수>";
                difficultyText.color= Color.limeGreen;
                break;
            case 2: // 급수 10 ~ 14
                difficultyText.text = "<중수>";
                difficultyText.color= Color.orange;
                break;
            case 1: // 급수 9 ~ 5
                difficultyText.text = "<고수>";
                difficultyText.color= Color.orangeRed;
                break;
            case 0: // 급수 1 ~ 4
                difficultyText.text = "<초고수>";
                difficultyText.color= Color.softRed;
                break;
        }
    }

    public void SetGameTurnPanel(GameTurnPanelType type) {
        switch (type) {
            case GameTurnPanelType.None:
                playerATurnPanel.SetActive(false);
                playerBTurnPanel.SetActive(false);
                aiLoadingPanel.SetActive(false);
                break;
            case GameTurnPanelType.ATurn:
                playerATurnPanel.SetActive(true);
                playerBTurnPanel.SetActive(false);
                aiLoadingPanel.SetActive(false);
                break;
            case GameTurnPanelType.BTurn:
                playerATurnPanel.SetActive(false);
                playerBTurnPanel.SetActive(true);
                if (_gameType == Constants.GameType.SinglePlay) // AI 턴일때 로딩 이미지 출력을 위한 조건문
                {
                    aiLoadingPanel.SetActive(true);
                }
                break;
        }
    }

    // 플레이어 급수 패널 설정
    public void SetPlayerRateTierPanel(GameTurnPanelType type, int rateTier, int currentEXP) {
        rateTierPanelController.SetPlayerRateTierPanel(type, rateTier, currentEXP);
    }
}
