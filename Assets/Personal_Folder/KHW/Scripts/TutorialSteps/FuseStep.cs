using System.Collections;
using UnityEngine;

public class FuseStep : TutorialStepBase
{
    [SerializeField] GameObject trainTriggerObject;

    void Start()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ActivateTrainCollider;
    }

    public override void ExecuteTutorial()
    {
        StartCoroutine(FuseTutorialCoroutine());
    }

    private IEnumerator FuseTutorialCoroutine()
    {
        TutorialTriggerUIController.Instance.ShowUI("6", 5f);

        GetComponent<BoxCollider>().enabled = false;

        ActivateTrainCollider();

        yield return new WaitForSeconds(5f);



        TutorialTriggerUIController.Instance.ShowUI("10", 5f);

        Destroy(gameObject);
    }

    private void ActivateTrainCollider()
    {
        trainTriggerObject.SetActive(true);
    }

    void OnDestroy()
    {
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= ActivateTrainCollider;        
    }
}
