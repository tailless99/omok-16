using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private GameObject confirmPanel;   // 확인창 패널
    [SerializeField] private GameObject signInPanel;    // 로그인 패널
    [SerializeField] private GameObject registerPanel;  // 회원가입 패널
    [SerializeField] private GameObject rematchPanel;

// 작성자 : 이동현
#region 멀티 모드 생성 후, 서버로 데이터 저장
    // TODO : 서버 연결 후, 플레이어 데이터를 서버에서 받아오도록 변경
    // 급수 저장 임시 변수
    public int rateTier = 18;  // 최하급 티어
    private int tierEXP = 0;    // 경험치
    public int haveGold = 0;   // 소지금

    // TODO : 서버 연결 후, 아이템 데이터 받아오도록 변경
    public bool isExpIncreaseActive = false; // 경험치 추가 증가 활성화 여부
    public bool isExpDecreaseActive = false; // 경험치 감소 활성화 여부

    // TODO : 로컬 플레이어 정보 => 서버 연결 후, 로그인한 아이디로 변경하기
    public Constants.PlayerType localPlayer = Constants.PlayerType.PlayerA;
#endregion

    private Constants.GameType _gameType;

    private int singlePlayWinningStreak = 0; //연승 저장 변수
    private int singlePlayBestCount = 0; // 최대 연승 저장
    private int dualPlayWinningStreak = 0; //연승 저장 변수
    private int dualPlayBestCount = 0; // 최대 연승 저장

    // Panel을 띄우기 위한 Canvas 할당
    private Canvas _canvas;

    // Game Logic
    private GameLogic _gameLogic;

    // Game 씬의 UI를 담당하는 객체
    private GameUIController _gameUIController;
    
    // 기보 시스템
    private ReplayController _replayController;


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
            
            
            _replayController = FindFirstObjectByType<ReplayController>();
        }
    }

    private void OnApplicationQuit() {
        _gameLogic = null;
    }



    //작성자: 이명호
    /// <summary>
    /// 연승횟수, 최다연승 횟수 가져오는 함수
    /// </summary>
    /// <param name="singlePlayWinningStreak"></param>
    /// <param name="singlePlayBestCount"></param>
    /// <param name="dualPlayWinningStreak"></param>
    /// <param name="dualPlayBestCount"></param>
    public void GetWinningStreak(out int singlePlayWinningStreak, out int singlePlayBestCount, out int dualPlayWinningStreak, out int dualPlayBestCount)
    {
        singlePlayWinningStreak = this.singlePlayWinningStreak;
        singlePlayBestCount = this.singlePlayBestCount;
        dualPlayWinningStreak = this.dualPlayWinningStreak;
        dualPlayBestCount = this.dualPlayBestCount;
    }


    //작성자: 이명호
    /// <summary>
    /// 연승횟수, 최다연승 정보 저장 함수
    /// </summary>
    /// <param name="singlePlayWinningStreak"></param>
    /// <param name="singlePlayBestCount"></param>
    /// <param name="dualPlayWinningStreak"></param>
    /// <param name="dualPlayBestCount"></param>
    public void SetWinningStreak(int singlePlayWinningStreak, int singlePlayBestCount, int dualPlayWinningStreak, int dualPlayBestCount)
    {
        this.singlePlayWinningStreak = singlePlayWinningStreak;
        this.singlePlayBestCount = singlePlayBestCount;
        this.dualPlayWinningStreak = dualPlayWinningStreak;
        this.dualPlayBestCount = dualPlayBestCount;
    }

    //작성자: 이명호
    /// <summary>
    /// 현재 게임타입 가져오는 함수
    /// </summary>
    /// <param name="_gameType"></param>
    public void GetGameType(out Constants.GameType _gameType)
    {
        _gameType = this._gameType;
    }

    public void GetTurnData()
    {
        var turnData = _gameLogic.GetTurnHistory();
        _replayController.GetReplayData(turnData);
    }

    // 턴 UI를 업데이트하는 함수
    public void UpdateTurnUI(int currentTurn, int totalTurn)
    {
        if (_replayController != null)
            _replayController.UpdateUI(currentTurn, totalTurn);
    }

    // 플레이 중 버튼을 숨기는 함수
    public void SetupReplayButtons(bool isON)
    {
        if (_replayController != null)
            _replayController.SetupButtons(isON);
    }

    public void OnReplayMode()
    {
        _gameLogic.IntoReplayMode();
    }

    public GameLogic GetGameLogic()
    {
        return _gameLogic;
    }

}
