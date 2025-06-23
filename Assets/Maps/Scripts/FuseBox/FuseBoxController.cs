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
        FindAnyObjectByType<UI_Electricity_Info>().GetComponent<Animator>().SetTrigger("On");
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
