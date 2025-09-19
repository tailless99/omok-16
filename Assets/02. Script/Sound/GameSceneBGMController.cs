using UnityEngine;
using System.Collections;

public class GameSceneBGMController : MonoBehaviour
{
    [Header("BGM 변경 시간 (초 단위)")]
    public float phase1Duration = 60f; // 0 ~ 1분
    public float phase2Duration = 60f; // 1분 ~ 2분
    // Phase3는 남은 시간 동안 계속 재생

    [Header("페이드 시간 (초 단위)")]
    public float fadeDuration = 0.8f;

    private float elapsedTime;
    private int currentPhase = -1;

    private void Start()
    {
        PlayPhase(0); // Phase1 시작
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (currentPhase == 0 && elapsedTime >= phase1Duration)
        {
            PlayPhase(1); // Phase2 시작
        }
        else if (currentPhase == 1 && elapsedTime >= phase1Duration + phase2Duration)
        {
            PlayPhase(2); // Phase3 시작
        }
        // Phase3는 조건 없이 유지 → 추가 처리 필요 없음
    }

    private void PlayPhase(int phase)
    {
        // 이미 재생 중이면 중복 호출 방지
        if (currentPhase == phase) return;

        currentPhase = phase;

        switch (phase)
        {
            case 0:
                StartCoroutine(CrossfadeTo(SoundType.BGM_In_Game1, fadeDuration));
                break;
            case 1:
                StartCoroutine(CrossfadeTo(SoundType.BGM_In_Game2, fadeDuration));
                break;
            case 2:
                StartCoroutine(CrossfadeTo(SoundType.BGM_In_Game3, fadeDuration));
                break;
        }
    }

    private IEnumerator CrossfadeTo(SoundType nextBgm, float duration)
    {
        AudioSource source = SoundManager.Instance.bgmSource;
        float startVolume = source.volume;

        // 페이드 아웃
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        // 새로운 BGM 재생
        SoundManager.Instance.PlayBGM(nextBgm);

        // 페이드 인
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, startVolume, t / duration);
            yield return null;
        }

        source.volume = startVolume; // 원래 볼륨 복원
    }
}