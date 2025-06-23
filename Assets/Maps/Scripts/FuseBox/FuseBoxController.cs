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
        Destroy(gameObject);
    }

    void OnDisable()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= DisableFuse;
    }
}
