using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class EndingSceneController : MonoBehaviour
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

    [Header("Door")]
    public Animator[] doorAnimators;
    public string openTriggerName = "Open";
    public float doorOpenDuration = 2f;

    [Header("BGM")]
    public AudioSource bgmAudioSource;
    public AudioClip titleBGM;
    [Tooltip("타이틀 BGM 페이드인 시간 (초)")]
    public float bgmFadeInTime = 2f;
    void Start()
    {
        if (subwayMovement != null)
            subwayMovement.SetSpeed(initialSpeed);
        StartCoroutine(PlayIntroSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        // 4) BGM 시작
        if (bgmAudioSource != null && titleBGM != null)
        {
            bgmAudioSource.clip = titleBGM;
            bgmAudioSource.volume = 0f;
            bgmAudioSource.Play();
            StartCoroutine(FadeAudioTo(bgmAudioSource, bgmFadeInTime, 1f));
        }

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
