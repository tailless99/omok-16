using UnityEngine;

public class MainPanelController : MonoBehaviour {
    public void OnClickSinglePlayButton() {
        // UI 사운드 추가
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlay);
    }

    public void OnClickDualPlayButton() {
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlay);
    }

    public void OnClickMultiPlayButton() {
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlay);
    }

    public void OnClickSettingsButton() { }
}