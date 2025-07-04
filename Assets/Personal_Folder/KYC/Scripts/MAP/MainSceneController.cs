using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 타이틀 화면 연출 컨트롤러
/// - 지하철 감속
/// - 브레이크 사운드
/// - 도어 열림 애니메이션
/// - UI 페이드 인
/// </summary>
public class MainSceneController : MonoBehaviour
{
    [Header("Subway Settings")]
    public SubwayMovement subwayMovement;
    public float initialSpeed = 10f;
    [Tooltip("감속에 걸릴 시간 (초)")]
    public float decelerationTime = 3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip brakeSound;
    public AudioClip doorOpenSound;

    [Header("BGM")]
    public AudioSource bgmAudioSource;
    public AudioClip titleBGM;
    [Tooltip("타이틀 BGM 페이드인 시간 (초)")]
    public float bgmFadeInTime = 2f;

    [Header("Door")]
    public Animator[] doorAnimators;
    public string openTriggerName = "Open";
    public float doorOpenDuration = 2f;

    [Header("UI")]
    public CanvasGroup titleUI;
    public GameObject logoImage;
    public GameObject[] buttons;
    public float buttonAppearDelay = 0.2f;
    public float uiFadeDuration = 0.5f;

    // 캐싱용
    private WaitForSeconds waitAfterDoor;
    private WaitForSeconds buttonDelay;
    private Dictionary<GameObject, CanvasGroup> buttonCanvasGroups = new();

    void Awake()
    {
        waitAfterDoor = new WaitForSeconds(doorOpenDuration);
        buttonDelay = new WaitForSeconds(buttonAppearDelay);

        // 버튼별 CanvasGroup 캐싱
        foreach (var btn in buttons)
        {
            if (btn != null)
            {
                var cg = btn.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    buttonCanvasGroups[btn] = cg;
                }
            }
        }
    }

    void Start()
    {
        // 초기 세팅
        if (subwayMovement != null)
            subwayMovement.SetSpeed(initialSpeed);

        titleUI.alpha = 0f;
        titleUI.interactable = titleUI.blocksRaycasts = false;

        if (logoImage != null)
        {
            var cg = logoImage.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = cg.blocksRaycasts = false;
                logoImage.SetActive(true);
            }
        }

        foreach (var kvp in buttonCanvasGroups)
        {
            kvp.Value.alpha = 0f;
            kvp.Value.interactable = kvp.Value.blocksRaycasts = false;
            kvp.Key.SetActive(true); // 미리 켜둠
        }

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // 1) 감속 사운드
        if (audioSource != null && brakeSound != null)
            audioSource.PlayOneShot(brakeSound);

        // 2) 감속
        float elapsed = 0f;
        while (elapsed < decelerationTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / decelerationTime);
            if (subwayMovement != null)
                subwayMovement.SetSpeed(Mathf.Lerp(initialSpeed, 0f, t));
            yield return null;
        }

        if (subwayMovement != null)
        {
            subwayMovement.SetSpeed(0f);
            subwayMovement.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        // 3) 도어 애니메이션
        foreach (var anim in doorAnimators)
            if (anim != null)
                anim.SetTrigger(openTriggerName);

        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.clip = doorOpenSound;
            audioSource.volume = 1f;
            audioSource.Play();
            StartCoroutine(FadeAudio(audioSource, doorOpenDuration));
        }

        yield return waitAfterDoor;

        // 4) BGM 시작
        if (bgmAudioSource != null && titleBGM != null)
        {
            bgmAudioSource.clip = titleBGM;
            bgmAudioSource.volume = 0f;
            bgmAudioSource.Play();
            StartCoroutine(FadeAudioTo(bgmAudioSource, bgmFadeInTime, 1f));
        }

        // 5) UI 페이드 인
        StartCoroutine(FadeInTitleUI());
    }

    private IEnumerator FadeInTitleUI()
    {
        titleUI.alpha = 1f;
        titleUI.interactable = titleUI.blocksRaycasts = true;

        // 로고 페이드 인
        if (logoImage != null)
        {
            var cg = logoImage.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = cg.blocksRaycasts = true;
                yield return StartCoroutine(FadeCanvasGroup(cg, 1.1f, 1f));
            }
        }

        yield return new WaitForSeconds(0.01f);

        // 버튼들 병렬 페이드 인
        foreach (var kvp in buttonCanvasGroups)
        {
            kvp.Value.interactable = kvp.Value.blocksRaycasts = true;
            StartCoroutine(FadeCanvasGroup(kvp.Value, uiFadeDuration, 1f));
            yield return buttonDelay; // 간격 유지
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float duration, float targetAlpha)
    {
        float startAlpha = group.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        group.alpha = targetAlpha;
    }

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

    public IEnumerator FadeAudioTo(AudioSource src, float duration, float targetVolume)
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
}
