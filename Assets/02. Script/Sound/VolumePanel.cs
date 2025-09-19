using UnityEngine;
using UnityEngine.UI;

public class VolumePanel : MonoBehaviour
{
    public static VolumePanel Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject panel;      // 볼륨 조절 패널 전체
    public Slider bgmSlider;      // BGM 볼륨 슬라이더
    public Slider uiSlider;       // UI 볼륨 슬라이더
    public Button closeButton;    // 닫기 버튼

    private bool isOpen = false; // 패널 상태 추적

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        float savedBgm = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float savedUi = PlayerPrefs.GetFloat("UI_Volume", 1f);

        bgmSlider.value = savedBgm;
        uiSlider.value = savedUi;
    }

    private void Start()
    {
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        uiSlider.onValueChanged.AddListener(OnUiVolumeChanged);

        ApplyVolumes();
        Close(); // 처음엔 닫혀있음

        // ✨ 여기서 버튼 클릭 이벤트 등록
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
        else
        {
            Debug.LogWarning("closeButton이 연결되지 않았습니다!");
        }
    }

    private void OnBgmVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBgmVolume(value);

        PlayerPrefs.SetFloat("BGM_Volume", value);
    }

    private void OnUiVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetUiVolume(value);

        PlayerPrefs.SetFloat("UI_Volume", value);
    }

    private void ApplyVolumes()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBgmVolume(bgmSlider.value);
            SoundManager.Instance.SetUiVolume(uiSlider.value);
        }
    }

    public void Open()
    {
        panel.SetActive(true);
        isOpen = true;
    }

    public void Close()
    {
        SoundManager.Instance.PlayUI(SoundType.UI_Click);
        panel.SetActive(false);
        isOpen = false;
        PlayerPrefs.Save();
    }
}