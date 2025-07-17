using UnityEngine;
using UnityEngine.Audio;

public class MixerSingleton : MonoBehaviour
{
    public static AudioMixer Mixer;
    public static AudioMixerGroup musicMixerGroup;
    public static AudioMixerGroup sfxMixerGroup;

    private void Awake()
    {
        // Resources/Mixer.mixer 에 AudioMixer 에셋을 넣어두세요
        Mixer = Resources.Load<AudioMixer>("Mixer");
        if (Mixer == null)
        {
            Debug.LogError("[MixerSingleton] Resources/Mixer.mixer 로드 실패");
            return;
        }

        // "Music" 그룹 할당
        var musicGroups = Mixer.FindMatchingGroups("Music");
        if (musicGroups != null && musicGroups.Length > 0)
            musicMixerGroup = musicGroups[0];
        else
            Debug.LogError("[MixerSingleton] Mixer에 'Music' 그룹이 없습니다");

        // "SFX" 그룹 할당
        var sfxGroups = Mixer.FindMatchingGroups("SFX");
        if (sfxGroups != null && sfxGroups.Length > 0)
            sfxMixerGroup = sfxGroups[0];
        else
            Debug.LogError("[MixerSingleton] Mixer에 'SFX' 그룹이 없습니다");

        DontDestroyOnLoad(gameObject);
    }

    // 필요 없으면 지우셔도 됩니다.
    private void Start() { }
    private void Update() { }
}
