using TMPro;
using UnityEngine;

public class ConfirmPanelController : PanelController {
    [SerializeField] private TextMeshProUGUI messageText;

    public delegate void OnConfirmButtonClicked();
    private OnConfirmButtonClicked _onConfirmButtonClicked;



    /// <summary>
    /// Confirm Panel을 표시하는 메서드
    /// </summary>
    /// <param name="message"></param>
    public void Show(string message, OnConfirmButtonClicked onConfirmButtonClicked) {
        messageText.text = message;
        _onConfirmButtonClicked = onConfirmButtonClicked;

        base.Show();
    }

    /// <summary>
    /// 확인 버튼 클릭 시, 호출되는 메서드
    /// </summary>
    public void OnClickConfirmButton() {
            SoundManager.Instance.PlayUI(SoundType.UI_Click);
        Hide(() => {
            _onConfirmButtonClicked?.Invoke();
        });

    }

    /// <summary>
    /// X 버튼 클릭 시, 호출되는 메서드
    /// </summary>
    public void OnClickCloseButton() {
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        Hide();
    }
}
