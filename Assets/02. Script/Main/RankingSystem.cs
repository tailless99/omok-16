using TMPro;
using UnityEngine;

//작성자: 이명호
//용도: 메인화면 사용자 급수 표시
public class RankingSystem : MonoBehaviour
{
    private int tierExp;
    private int rateTier;
    private int expPercent;
    public TextMeshProUGUI rankingText;
    public void Start()
    {
        GameManager.Instance.GetTierInfo(out rateTier, out tierExp);
        Ranking();
    }
    public void Ranking()
    {
        var requireExp = rateTier >= 10 ?
        Constants.minTierExp : rateTier >= 5 ?
        Constants.middleTierExp : Constants.maxTierExp;

        //남은 경험치 퍼센트로 나타내기
        var percent = ((float)tierExp / requireExp) * 100f;
        expPercent = (int) percent;

        rankingText.text = $"급수: {rateTier}급 {expPercent}%";
    }


}
