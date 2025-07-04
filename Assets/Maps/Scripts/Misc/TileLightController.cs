using UnityEngine;

public class TileLightController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GamePlayManager.instance.OnDangerAction += LightOff;
    }

    void OnDestroy()
    {
        GamePlayManager.instance.OnDangerAction -= LightOff;
    }

    private void LightOff()
    {
        foreach (var light in GetComponentsInChildren<Light>())
        {
            light.color = Color.red;
        }
    }
}
