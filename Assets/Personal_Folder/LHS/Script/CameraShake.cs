using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    
    private CinemachineBasicMultiChannelPerlin _perlin;

    private float _shakeTimer;
    private float _shakeDuration;
    private float _startingIntensity;

    private void Awake()
    {
        Instance = this;
        _perlin = GetComponent<CinemachineVirtualCamera>()
    .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    public void ShakeCamera(float intensity, float duration)
    {
        _perlin.m_AmplitudeGain = intensity;
        _shakeDuration = duration;
        _shakeTimer = duration;
        _startingIntensity = intensity;
    }

    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0f)
            {
                _perlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
