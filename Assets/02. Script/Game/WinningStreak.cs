using TMPro;
using UnityEngine;

//작성자: 이명호
//용도: 인게임 연승, 최다연승 표시
public class WinningStreak : MonoBehaviour
{
    public Constants.GameType _gameType;
    public GameLogic gameLogic;
    private int singlePlayWinningStreak;
    private int singlePlayBestCount;
    private int dualPlayWinningStreak;
    private int dualPlayBestCount;
    public TextMeshProUGUI winningStreakText;

    public void Start()
    {
        GameManager.Instance.GetGameType(out _gameType);
        GameManager.Instance.GetWinningStreak(out singlePlayWinningStreak, out singlePlayBestCount, out dualPlayWinningStreak, out dualPlayBestCount);
        WinningText();
    }

    //작성자: 이명호
    /// <summary>
    /// 싱글, 듀얼 플레이 연승, 최다 연승 계산 함수
    /// </summary>
    /// <param name="result"></param>
    public void WinningCount(GameLogic.GameResult result)
    {
        if (_gameType == Constants.GameType.SinglePlay && result == GameLogic.GameResult.PlayerAWin)
        {
            singlePlayWinningStreak += 1;
            if (singlePlayWinningStreak >= singlePlayBestCount)
            {
                singlePlayBestCount = singlePlayWinningStreak;
            }
        }
        else if (_gameType == Constants.GameType.SinglePlay && result == GameLogic.GameResult.PlayerBWin)
        {
            singlePlayWinningStreak = 0;
        }


        else if (_gameType == Constants.GameType.DualPlay && result == GameLogic.GameResult.PlayerAWin)
        {
            dualPlayWinningStreak += 1;
            if (dualPlayWinningStreak >= dualPlayBestCount)
            {
                dualPlayBestCount = dualPlayWinningStreak;
            }
        }
        else if (_gameType == Constants.GameType.DualPlay && result == GameLogic.GameResult.PlayerBWin)
        {
            dualPlayWinningStreak = 0;
        }

        GameManager.Instance.SetWinningStreak(singlePlayWinningStreak, singlePlayBestCount, dualPlayWinningStreak, dualPlayBestCount);
        WinningText();
    }

    /// <summary>
    /// 연승, 최다연승 ui표시 함수
    /// </summary>
    public void WinningText()
    {
        if (_gameType == Constants.GameType.SinglePlay) 
        {
            winningStreakText.text = $"<color=#FFD700>연승:</color>{singlePlayWinningStreak}연승\n<color=#E30000>최고 연승:</color>{singlePlayBestCount}연승";
        }
        else if (_gameType == Constants.GameType.DualPlay)
        {
            winningStreakText.text = $"<color=#FFD700>연승:</color>{dualPlayWinningStreak}연승\n<color=#E30000>최고 연승:</color>{dualPlayBestCount}연승";
        }
    }


}
