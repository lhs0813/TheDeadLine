using UnityEngine;

public class MixerRegisterer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var sources = GetComponents<AudioSource>();
        foreach (var src in sources)
        {
            src.outputAudioMixerGroup = MixerSingleton.sfxMixerGroup;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
