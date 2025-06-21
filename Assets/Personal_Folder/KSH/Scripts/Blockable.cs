using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.Audio;

public class Blockable : MonoBehaviour
{
    public Vector3 ShakeCamera;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public void Block()
    {
        var clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.clip = clip;
        audioSource.Play();

        FindAnyObjectByType<CameraManager>().ShakeCameras(ShakeCamera.x, ShakeCamera.y, ShakeCamera.z);
    }
}
