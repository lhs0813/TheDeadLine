using UnityEngine;

public class MixerRegisterer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<AudioSource>().outputAudioMixerGroup = MixerSingleton.sfxMixerGroup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
