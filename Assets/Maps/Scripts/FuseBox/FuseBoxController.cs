using Akila.FPSFramework;
using UnityEngine;

public class FuseBoxController : MonoBehaviour
{
    void Start()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += DisableFuse;
    }

    public void OnFuseActivated()
    {
        ObjectiveManager.instance.FuseFound();
        gameObject.SetActive(false);
    }

    void DisableFuse()
    {
        Debug.Log("Fuse Disabled!");
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= DisableFuse;
    }
}
