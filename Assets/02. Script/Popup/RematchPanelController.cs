using TMPro;
using UnityEngine;

public class RematchPanelController : PanelController {
    [SerializeField] private TextMeshProUGUI messageText;

    public delegate void OnRematchButtonClicked();
    private OnRematchButtonClicked _onRematchButtonClicked;



    /// <summary>
    /// Rematch Panel을 표시하는 메서드
    /// </summary>
    /// <param name="message"></param>
    public void Show(string message, OnRematchButtonClicked onRematchButtonClicked) {
        messageText.text = message;
        _onRematchButtonClicked = onRematchButtonClicked;

        base.Show();
    }

    /// <summary>
    /// 확인 버튼 클릭 시, 호출되는 메서드
    /// </summary>
    public void OnClickRematchButton() {
            SoundManager.Instance.PlayUI(SoundType.UI_Click);
        Hide(() => {
            _onRematchButtonClicked?.Invoke();
        });

    }
    
    public void OnClickCloseButton() {
        Hide();
    }

    /// <summary>
    /// X 버튼 클릭 시, 호출되는 메서드
    /// </summary>
    public void OnClickReplayButton() {
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        Hide();
        GameManager.Instance.OnReplayMode();
    }
}