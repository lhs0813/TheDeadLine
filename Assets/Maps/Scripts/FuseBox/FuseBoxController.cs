using Akila.FPSFramework;
using UnityEngine;

public class FuseBoxController : MonoBehaviour
{
    void OnEnable()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += DisableFuse;
    }

    public void OnFuseActivated()
    {
        ObjectiveManager.instance.FuseFound();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void DisableFuse()
    {
        Debug.Log("Fuse Disabled!");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= DisableFuse;
    }
}
