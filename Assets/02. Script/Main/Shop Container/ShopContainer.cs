using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 상점 시스템 컨테이너
/// 작성자 : 이동현
/// </summary>
public class ShopContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI haveGoldText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private SellSlot[] sellSlots; // 판매 슬롯 배열

    private void OnEnable() {
        // 초기화
        haveGoldText.text = GameManager.Instance.haveGold.ToString();
        costText.text = "0";

        // slot의 이벤트 구독
        foreach (SellSlot slot in sellSlots) {
            slot.btnHoverEvent += (cost) => SetCostPanelText(cost);
            slot.BuyItemEvent += SetHaveGoldPanelText;
        }
    }

    private void OnDisable() {
        // slot의 이벤트 구독 해제
        foreach (SellSlot slot in sellSlots) {
            slot.btnHoverEvent -= (cost) => SetCostPanelText(cost);
            slot.BuyItemEvent -= SetHaveGoldPanelText;
        }
    }

    // 버튼을 호버했을 때 실행되는 콜백 메서드
    void SetCostPanelText(int cost) {
        if(cost == 0) {
            costText.text = "";
            return;
        }
        costText.text = cost.ToString();
    }

    // 아이템 구매했을 때 실행되는 콜백 메서드
    void SetHaveGoldPanelText() {
        haveGoldText.text = GameManager.Instance.haveGold.ToString();
    }
}
