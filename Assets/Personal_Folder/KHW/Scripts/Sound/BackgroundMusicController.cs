using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicController : MonoBehaviour
{
    [Tooltip("Addressables에서 불러온 전투 BGM 리스트")]
    public List<AudioClip> backgroundMusicsOnCombat = new List<AudioClip>();
    public List<AudioClip> backgroundMusicsOnDanger = new List<AudioClip>();

    [Tooltip("페이드 인/아웃에 걸릴 시간(초)")]
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource _audioSrc;
    private Coroutine _fadeCoroutine;

    private async void Start()
    {
        _audioSrc = GetComponent<AudioSource>();
        await LoadBackgroundMusicsOnCombat();
        await LoadBackgroundMusicsOnDanger();
    }

    async Task LoadBackgroundMusicsOnCombat()
    {
        var handle = Addressables.LoadAssetsAsync<AudioClip>("BGM_Combat", null);
        var flow = await handle.Task;

        if (flow == null)
        {
            Debug.LogError($"Addressables 라벨 'BGM_Combat' 에 AudioClip이 없습니다.");
            return;
        }

        backgroundMusicsOnCombat = handle.Result.ToList();
    }

    async Task LoadBackgroundMusicsOnDanger()
    {
        var handle = Addressables.LoadAssetsAsync<AudioClip>("BGM_Danger", null);
        var flow = await handle.Task;

        if (flow == null)
        {
            Debug.LogError($"Addressables 라벨 'BGM_Danger' 에 AudioClip이 없습니다.");
            return;
        }

        backgroundMusicsOnDanger = handle.Result.ToList();
    }

    /// <summary>
    /// 배경음악 리스트에서 랜덤으로 하나 골라 페이드 인하며 재생합니다.
    /// </summary>
    public void PlayRandomCombatMusic()
    {
        if (backgroundMusicsOnCombat.Count == 0)
        {
            Debug.LogWarning("전투 BGM이 로드되지 않았습니다.");
            return;
        }

        // 랜덤으로 클립 선택
        AudioClip clip = backgroundMusicsOnCombat[Random.Range(0, backgroundMusicsOnCombat.Count)];
        _audioSrc.clip = clip;
        _audioSrc.volume = 0f;
        _audioSrc.Play();

        // 이전 페이드 코루틴 정리
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeInCoroutine(fadeDuration));
    }

    public void PlayRandomDangerMusic()
    {
        if (backgroundMusicsOnDanger.Count == 0)
        {
            Debug.LogWarning("전투 BGM이 로드되지 않았습니다.");
            return;
        }

        // 랜덤으로 클립 선택
        AudioClip clip = backgroundMusicsOnDanger[Random.Range(0, backgroundMusicsOnDanger.Count)];
        _audioSrc.Stop();
        _audioSrc.clip = clip;
        _audioSrc.volume = 1f;
        _audioSrc.Play();

        // 이전 페이드 코루틴 정리
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeInCoroutine(fadeDuration));
    }

    



    /// <summary>
    /// 현재 재생 중인 음악을 페이드 아웃하며 중지합니다.
    /// </summary>
    public void StopCombatMusic(float _fadeDuration = 5f)
    {
        if (!_audioSrc.isPlaying) return;

        if (_fadeDuration != 5)
            fadeDuration = _fadeDuration;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutCoroutine(2f));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _audioSrc.volume = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        _audioSrc.volume = 1f;
        _fadeCoroutine = null;
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVol = _audioSrc.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _audioSrc.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        _audioSrc.volume = 0f;
        _audioSrc.Stop();
        _fadeCoroutine = null;
    }
}
