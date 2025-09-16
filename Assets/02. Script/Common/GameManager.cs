using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private GameObject confirmPanel;   // 확인창 패널
    [SerializeField] private GameObject signInPanel;    // 로그인 패널
    [SerializeField] private GameObject registerPanel;  // 회원가입 패널
    [SerializeField] private GameObject rematchPanel;

    // 급수 저장 임시 변수
    // 멀티 모드 생성 후, 서버로 데이터 저장
    private int rateTier = 18;  // 최하급 티어
    private int tierEXP = 0;    // 경험치
    public int haveGold = 0;   // 소지금
    public bool isExpIncreaseActive = false; // 경험치 추가 증가 활성화 여부
    public bool isExpDecreaseActive = false; // 경험치 감소 활성화 여부

    private Constants.GameType _gameType;

    // Panel을 띄우기 위한 Canvas 할당
    private Canvas _canvas;

    // Game Logic
    private GameLogic _gameLogic;

    // Game 씬의 UI를 담당하는 객체
    private GameUIController _gameUIController;


    /// <summary>
    /// PlayerState에서 급수 정보를 설정하기 위한 메서드
    /// 멀티 서버 생성 후, 삭제할 것. 정보는 서버에서 저장
    /// </summary>
    /// <param name="rateTier"></param>
    /// <param name="tierEXP"></param>
    public void SetTierInfo(int rateTier, int tierEXP, int gold) { 
        this.rateTier = rateTier;
        this.tierEXP = tierEXP;
        this.haveGold += gold;
    }

    /// <summary>
    /// 플레이어의 급수 정보를 가져오는 메서드
    /// </summary>
    /// <returns></returns>
    public void GetTierInfo(out int tier, out int tierExp) {
        tier = this.rateTier;
        tierExp = this.tierEXP;
    }

    /// <summary>
    /// Main에서 Game Scene으로 전환시 호출될 메서드
    /// </summary>
    public void ChangeToGameScene(Constants.GameType gameType) {
        _gameType = gameType;
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Game에서 Main Scene으로 전환 시 호출될 메서드
    /// </summary>
    public void ChangeToMainScene() {
        _gameLogic = null;
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Confirm Panel을 띄우는 메서드
    /// </summary>
    /// <param name="message"></param>
    public void OpenConfirmPanel(string message, ConfirmPanelController.OnConfirmButtonClicked onConfirmButtonClicked) {
        if (_canvas != null) {
            var confirmPanelObject = Instantiate(confirmPanel, _canvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, onConfirmButtonClicked);
        }
    }
    /// <summary>
    /// RematchPanel을 여는 메서드
    /// </summary>
    public void OpenRematchPanel(string message, RematchPanelController.OnRematchButtonClicked onRematchButtonClicked)
    {
        if (_canvas != null)
        {
            var rematchPanelObject = Instantiate(rematchPanel, _canvas.transform);
            rematchPanelObject.GetComponent<RematchPanelController>().Show(message, onRematchButtonClicked);
        }
    }

    /// <summary>
    /// Game Scene에서 턴을 표시하는 UI를 제어하는 함수
    /// </summary>
    /// <param name="type"></param>
    public void SetGameTurnPanel(GameUIController.GameTurnPanelType type) {
        _gameUIController.SetGameTurnPanel(type);
    }

    /// <summary>
    /// Game Scene에서 플레이어의 급수를 표시하는 UI를 제어하는 함수
    /// </summary>
    /// <param name="type"></param>
    public void SetPlayerRateTierPanel(GameUIController.GameTurnPanelType type, int rateTier, int currentEXP) {
        _gameUIController.SetPlayerRateTierPanel(type, rateTier, currentEXP);
    }

    // 씬 로드시 호출되는 함수
    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        _canvas = FindFirstObjectByType<Canvas>();
        
        if (scene.name == "Game") {
            // Block 초기화
            var blockContoroller = FindFirstObjectByType<BlockController>();
            if (blockContoroller != null) {
                blockContoroller.InitBlocks();
            }
            
            // Game UI Controller 할당 및 초기화
            _gameUIController = FindFirstObjectByType<GameUIController>();
            if (_gameUIController != null) {
                _gameUIController.SetGameTurnPanel(GameUIController.GameTurnPanelType.None);
            }
            
            // GameLogic 생성
            if (_gameLogic == null) {
                _gameLogic = new GameLogic(blockContoroller, _gameType);
            }
        }
    }

    private void OnApplicationQuit() {
        _gameLogic = null;
    }
}
