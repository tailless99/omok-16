using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public enum SellItemType { None, ExpIncreased, ExpDecreased }
    public SellItemType sellItemType;
    
    public int sellCost;
    public Action<int> btnHoverEvent;
    public Action BuyItemEvent;

    // 호버 이벤트
    public void OnPointerEnter(PointerEventData eventData) {
        btnHoverEvent?.Invoke(sellCost);
    }

    // 호버 아웃 이벤트
    public void OnPointerExit(PointerEventData eventData) {
        btnHoverEvent?.Invoke(0);
    }

    // 버튼이 클릭되었을 때 호출되는 메서드
    public void OnBtnClickCallBack() {
        var haveGold = GameManager.Instance.haveGold;

        if(haveGold >= sellCost) {
            GameManager.Instance.haveGold -= sellCost;
            GameManager.Instance.OpenConfirmPanel($"{sellCost} 골드를 지불하고 아이템을 구매했습니다.", (() => {
                BuyItemEvent?.Invoke();

                if (sellItemType == SellItemType.ExpIncreased) {
                    GameManager.Instance.isExpIncreaseActive = true;
                }
                else if (sellItemType == SellItemType.ExpDecreased) {
                    GameManager.Instance.isExpDecreaseActive = true;
                }
            }));
        }
        else {
            GameManager.Instance.OpenConfirmPanel("골드가 부족합니다.", (() => { }));
        }
    }
}