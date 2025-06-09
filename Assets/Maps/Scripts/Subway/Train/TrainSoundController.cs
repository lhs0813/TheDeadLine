using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TrainSoundController : MonoBehaviour
{
    private AudioSource trainAudioSource;

    private List<AudioClip> trainDepartingClips   = new List<AudioClip>();
    private List<AudioClip> trainArrivingClips    = new List<AudioClip>();
    private List<AudioClip> trainRunningClips     = new List<AudioClip>();
    private List<AudioClip> doorOpenClips         = new List<AudioClip>();
    private List<AudioClip> doorCloseClips        = new List<AudioClip>();

    async void Start()
    {
        trainAudioSource = GetComponent<AudioSource>();
        await InitializeAllClipsAsync();
    }

    private async Task InitializeAllClipsAsync()
    {
        // “SubwaySound” 라벨로 모든 지하철 관련 AudioClip 로드
        var handle = Addressables.LoadAssetsAsync<AudioClip>(
            "SubwaySound",
            (clip) => { /* 로드 중에 필요하다면 콜백 */ }
        );
        await handle.Task;
        List<AudioClip> allClips = handle.Result.ToList();

        // 이름 컨벤션에 따라 분류
        trainDepartingClips = allClips
            .Where(c => c.name.ToLower().Contains("departing"))
            .ToList();

        trainArrivingClips = allClips
            .Where(c => c.name.ToLower().Contains("arriv"))
            .ToList();

        Debug.Log($"Arriving clip count: {trainArrivingClips.Count}");

        trainRunningClips = allClips
            .Where(c => c.name.ToLower().Contains("running"))
            .ToList();

        doorOpenClips = allClips
            .Where(c => c.name.ToLower().Contains("dooropen"))
            .ToList();

        doorCloseClips = allClips
            .Where(c => c.name.ToLower().Contains("doorclose"))
            .ToList();
    }

    // 아래는 사용 예시입니다

    public void PlayDoorOpen()
    {
        trainAudioSource.Stop();  
        if (doorOpenClips.Count == 0) return;
        var clip = doorOpenClips[Random.Range(0, doorOpenClips.Count)];
        trainAudioSource.PlayOneShot(clip);
    }

    public void PlayDoorClose()
    {
        trainAudioSource.Stop();  
        if (doorCloseClips.Count == 0) return;
        var clip = doorCloseClips[Random.Range(0, doorCloseClips.Count)];
        trainAudioSource.PlayOneShot(clip);
    }

    public void PlayTrainRunning()
    {
        if (trainRunningClips.Count == 0) return;
        trainAudioSource.loop = true;
        trainAudioSource.clip = trainRunningClips[Random.Range(0, trainRunningClips.Count)];
        trainAudioSource.Play();
    }

    public void PlayTrainStopping()
    {
        
        trainAudioSource.loop = false;
        trainAudioSource.Stop();
    }

    public void PlayTrainDeparting()
    {
        if (trainDepartingClips.Count == 0) return;
        var clip = trainDepartingClips[Random.Range(0, trainDepartingClips.Count)];
        trainAudioSource.PlayOneShot(clip);
    }

    public void PlayTrainArriving()
    {
        if (trainArrivingClips.Count == 0) return;
        var clip = trainArrivingClips[Random.Range(0, trainArrivingClips.Count)];
        trainAudioSource.PlayOneShot(clip);
    }
}
