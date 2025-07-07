using System.Collections;
using UnityEngine;

public class GunPickUpTutorialStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        StartCoroutine(GunTutorialCoroutine());
    }

    private IEnumerator GunTutorialCoroutine()
    {
        TutorialTriggerUIController.Instance.ShowUI("1", 5f);

        yield return new WaitForSeconds(5f);

        TutorialTriggerUIController.Instance.ShowUI("2", 5f);
        
        Destroy(gameObject);
    }

}
