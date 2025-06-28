// MainSceneController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 타이틀 화면 진입 시 연출을 담당하는 컨트롤러
/// - 지하철 감속
/// - 브레이크 사운드 재생
/// - 도어 오픈 애니메이션
/// - 타이틀 UI 페이드 인
/// </summary>
public class MainSceneController : MonoBehaviour
{
    [Header("Subway Settings")]
    public SubwayMovement subwayMovement;      // SubwayMovement 컴포넌트 참조
    public float initialSpeed = 10f;          // 시작 속도
    [Tooltip("감속에 걸릴 시간 (초)")]
    public float decelerationTime = 3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip brakeSound;             // 브레이크(감속) 사운드
    public AudioClip doorOpenSound;

    [Header("BGM")]
    public AudioSource bgmAudioSource;    // 배경 음악 전용 AudioSource
    public AudioClip titleBGM;
    [Tooltip("타이틀 BGM 페이드인 시간 (초)")]
    public float bgmFadeInTime = 2f;

    [Header("Door")]
    public Animator[] doorAnimators;          // 도어 애니메이터
    public string openTriggerName = "Open"; // 도어 열기 트리거 이름
    [Tooltip("도어 오픈 애니메이션 재생 대기 시간 (초)")]
    public float doorOpenDuration = 2f;

    [Header("UI")]
    public CanvasGroup titleUI;              // 로고 + Start 버튼이 포함된 CanvasGroup

    void Start()
    {
        // 지하철 시작 속도 설정
        if (subwayMovement != null)
            subwayMovement.SetSpeed(initialSpeed);

        // 타이틀 UI 비활성화
        titleUI.alpha = 0f;
        titleUI.interactable = titleUI.blocksRaycasts = false;

        // 연출 코루틴 시작
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // 1) 감속 사운드 재생
        if (audioSource != null && brakeSound != null)
            audioSource.PlayOneShot(brakeSound);

        // 2) decelerationTime 동안 속도 선형 감속
        float elapsed = 0f;
        while (elapsed < decelerationTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / decelerationTime);
            float currentSpeed = Mathf.Lerp(initialSpeed, 0f, t);
            if (subwayMovement != null)
                subwayMovement.SetSpeed(currentSpeed);

            yield return null;
        }

        // 완전 정지
        if (subwayMovement != null)
        {
            subwayMovement.SetSpeed(0f);
            subwayMovement.enabled = false;
        }
        else
        {
            Debug.LogError("SubwayMovement 컴포넌트가 할당되지 않았습니다!");
        }

        // 3) 도어 열기 트리거
        foreach (var anim in doorAnimators)
            if (anim != null)
                anim.SetTrigger(openTriggerName);

        // 4) 도어 오픈 사운드 재생 & 페이드 아웃
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.clip = doorOpenSound;
            audioSource.volume = 1f;
            audioSource.Play();
            StartCoroutine(FadeAudio(audioSource, doorOpenDuration));
        }

        // 4) 도어 애니메이션 대기
        yield return new WaitForSeconds(doorOpenDuration);

        // 5) 타이틀 BGM 페이드인
        if (bgmAudioSource != null && titleBGM != null)
        {
            bgmAudioSource.clip = titleBGM;
            bgmAudioSource.volume = 0f;
            bgmAudioSource.Play();
            StartCoroutine(FadeAudioTo(bgmAudioSource, bgmFadeInTime, 1f));
        }

        // 6) 타이틀 UI 페이드 인
        StartCoroutine(FadeInTitleUI());
    }

    // 브레이크/도어 사운드 페이드아웃
    private IEnumerator FadeAudio(AudioSource src, float duration)
    {
        float startVol = src.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            src.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        src.volume = 0f;
    }

    // BGM 페이드인
    private IEnumerator FadeAudioTo(AudioSource src, float duration, float targetVolume)
    {
        float startVol = src.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            src.volume = Mathf.Lerp(startVol, targetVolume, elapsed / duration);
            yield return null;
        }
        src.volume = targetVolume;
    }

    private IEnumerator FadeInTitleUI()
    {
        titleUI.interactable = titleUI.blocksRaycasts = true;
        float fadeTime = 3f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);
            float alpha;
            if (t < 0.5f)
            {
                // Ease-in quadratic: first half
                float u = t / 0.5f;
                alpha = 0.5f * u * u;
            }
            else
            {
                // Ease-out quadratic: second half
                float u = (t - 0.5f) / 0.5f;
                alpha = 0.5f + (1f - (1f - u) * (1f - u)) * 0.5f;
            }
            titleUI.alpha = alpha;
            yield return null;
        }
        titleUI.alpha = 1f;

    }
}
