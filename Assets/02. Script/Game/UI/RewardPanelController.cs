using DG.Tweening;
using TMPro;
using UnityEngine;

public class RewardPanelController : MonoBehaviour
{
    // 랭크업 텍스트 
    [SerializeField] TextMeshProUGUI rankUPText;  // 랭크업 안내 텍스트
    [SerializeField] TextMeshProUGUI rankText;    // 현재 랭크 텍스트

    // 경험치 텍스트
    [SerializeField] TextMeshProUGUI expText;

    // 골드 텍스트
    [SerializeField] TextMeshProUGUI goldText;

    public void InitUI(int prevExp, int nextExp, int prevGold, int nextGold, Constants.RankChangeType rankType) {
        // 초기화
        expText.text = $"{prevExp} => {nextExp}";
        goldText.text = $"{prevGold} => {nextGold}";

        rankText.text = $"{GameManager.Instance.rateTier}급";
        var rankUptext = rankType == Constants.RankChangeType.RankUp ? "Rank Up" : rankType == Constants.RankChangeType.RankDown ? "Rank Down" : "";
        rankUPText.text = rankUptext;

        // 오브젝트 활성화
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;

        // DoTween을 이용한 Fade
        transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
}
