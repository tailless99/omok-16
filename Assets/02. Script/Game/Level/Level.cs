using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


//작성자: 이명호
//용도: 급수 시스템 적용
public class Level : MonoBehaviour
{
    public GameObject levelUI;
    public static int score;
    private Constants.PlayerType[,] _board;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; //씬 변경될떄 OnSceneLoaded함수 동작
    }
    private void Start()
    {
        //PlayerPrefs.DeleteAll(); 저장된 값 모두 삭제

        score = PlayerPrefs.GetInt("PlayerScore", 0); // 저장된 score값 불러오기 저장된 값 없으면 기본값 0
        FindPlayerLevel();
    }

    //작성자: 이명호
    //용도: 게임에서 이기거나 질때 3게임마다 급수 증감
    /// <summary>
    /// 급수 증감 함수
    /// </summary>
    public void PlayerALevel()
    {
        int level = Mathf.Clamp(9 - (score / 30), 1, 9);

        if (levelUI == null) return;

        levelUI.GetComponent<TextMeshProUGUI>().text = $"급수: {level}급";

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerLevel();
    }

    //작성자: 이명호
    //용도: 급수 TextUI 찾아서 적용하는 기능
    /// <summary>
    /// 급수 TextUI 찾아서 적용
    /// </summary>
    private void FindPlayerLevel()
    {
        levelUI = GameObject.FindWithTag("Level");
        score = Mathf.Clamp(score, 0, 270);
        PlayerALevel();
    }
    //작성자: 이명호
    //용도: score 디스크에 저장
    /// <summary>
    /// score + 10 디스크에 저장
    /// </summary>
    public static void AddScoreSave()
    {
        score += 10;
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();

    }
    //작성자: 이명호
    //용도: score 디스크에 저장
    /// <summary>
    /// score - 10 디스크에 저장
    /// </summary>
    public static void RemoveSaveScore()
    {
        score -= 10;
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();

    }

}
