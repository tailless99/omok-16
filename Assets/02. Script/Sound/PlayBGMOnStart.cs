using UnityEngine;

public class PlayBGMOnStart : MonoBehaviour
{
    public SoundType bgmType;

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmType);
        }
    }
}