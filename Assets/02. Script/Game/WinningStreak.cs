using TMPro;
using UnityEngine;

//작성자: 이명호
//용도: 인게임 연승, 최다연승 표시
public class WinningStreak : MonoBehaviour
{
    public Constants.GameType _gameType;
    public GameLogic gameLogic;
    private int winningStreak;
    private int bestCount;
    public TextMeshProUGUI winningStreakText;

    public void Start()
    {
        GameManager.Instance.GetGameType(out _gameType);
        GameManager.Instance.GetWinningStreak(out winningStreak, out bestCount);
        WinningText();
    }

    //작성자: 이명호
    /// <summary>
    /// 싱글 플레이 연승, 최다 연승 계산 함수
    /// </summary>
    /// <param name="result"></param>
    public void WinningCount(GameLogic.GameResult result)
    {
        if (_gameType == Constants.GameType.SinglePlay && result == GameLogic.GameResult.PlayerAWin)
        {
            winningStreak += 1;
            if (winningStreak >= bestCount)
            {
                bestCount = winningStreak;
            }
        }
        else if (_gameType == Constants.GameType.SinglePlay && result == GameLogic.GameResult.PlayerBWin)
        {
            winningStreak = 0;
        }

        GameManager.Instance.SetWinningStreak(winningStreak, bestCount);
    }

    /// <summary>
    /// 연승, 최다연승 ui표시 함수
    /// </summary>
    public void WinningText()
    {
        if (_gameType == Constants.GameType.SinglePlay)
        {
            winningStreakText.text = $"<color=#FFD700>연승:</color>{winningStreak}연승\n<color=#E30000>최고 연승:</color>{bestCount}연승";
        }
    }


}
