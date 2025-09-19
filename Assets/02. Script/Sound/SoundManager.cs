using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public enum SoundType
{
    // BGM
    BGM_Title,
    BGM_In_Game1,
    BGM_In_Game2,
    BGM_In_Game3,
    BGM_Game_Over,

    // UI
    UI_Click,
    UI_Set_Baduk
}

[System.Serializable]
public class SoundData
{
    public SoundType type;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Data")]
    public List<SoundData> soundDatas = new List<SoundData>();

    [Header("Audio Sources")]
    public AudioSource bgmSource;  // BGM 재생용
    public AudioSource uiSource;   // UI 재생용

    private Dictionary<SoundType, AudioClip> soundDict;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    private void Awake()
    {
        // 싱글톤 보장
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Dictionary 생성
        soundDict = new Dictionary<SoundType, AudioClip>();
        foreach (var data in soundDatas)
        {
            if (!soundDict.ContainsKey(data.type))
                soundDict.Add(data.type, data.clip);
        }

        // 저장된 볼륨 불러오기
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        uiVolume = PlayerPrefs.GetFloat("UI_Volume", 1f);

        ApplyVolumes();
    }

    /// <summary>
    /// BGM 재생 (즉시 전환)
    /// </summary>
    public void PlayBGM(SoundType type)
    {
        if (!soundDict.ContainsKey(type)) return;

        AudioClip clip = soundDict[type];
        if (bgmSource.clip == clip) return; // 같은 곡이면 무시

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// UI 효과음 재생 (중첩 가능)
    /// </summary>
    public void PlayUI(SoundType type)
    {
        if (!soundDict.ContainsKey(type)) return;
        uiSource.PlayOneShot(soundDict[type], uiVolume);
    }

    /// <summary>
    /// BGM 볼륨 변경
    /// </summary>
    public void SetBgmVolume(float value)
    {
        bgmVolume = value;
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        PlayerPrefs.SetFloat("BGM_Volume", value);
    }

    /// <summary>
    /// UI 볼륨 변경
    /// </summary>
    public void SetUiVolume(float value)
    {
        uiVolume = value;
        if (uiSource != null)
            uiSource.volume = uiVolume;

        PlayerPrefs.SetFloat("UI_Volume", value);
    }

    /// <summary>
    /// 저장된 볼륨 값 적용
    /// </summary>
    private void ApplyVolumes()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (uiSource != null) uiSource.volume = uiVolume;
    }
}