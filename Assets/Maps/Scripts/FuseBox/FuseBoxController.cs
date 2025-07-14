using Akila.FPSFramework;
using UnityEngine;

public class FuseBoxController : MonoBehaviour
{
    void OnEnable()
    {
        if (ObjectiveManager.instance != null)
            ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += DisableFuse;
    }

    public void OnFuseActivated()
    {
        ObjectiveManager.instance.FuseFound();
        gameObject.SetActive(false);  // → OnDisable() 호출 → 언구독
    }

    void DisableFuse()
    {
        gameObject.SetActive(false);  // → OnDisable() 호출 → 언구독
    }

    void OnDisable()
    {
        if (ObjectiveManager.instance != null)
            ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= DisableFuse;
    }

    // 만약 파괴 순서가 불안정하다면 추가로…
    void OnDestroy()
    {
        if (ObjectiveManager.instance != null)
            ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= DisableFuse;
    }
}
