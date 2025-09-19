using UnityEngine;

public class SubPannelController : MonoBehaviour
{
    public void OnOptionsButtonClicked()
    {
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        VolumePanel.Instance.Open();
    }
}