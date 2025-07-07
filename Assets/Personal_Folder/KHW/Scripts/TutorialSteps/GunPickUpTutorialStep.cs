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
        TutorialTriggerUIController.Instance.ShowUI("1", 3f);

        yield return new WaitForSeconds(3f);

        TutorialTriggerUIController.Instance.ShowUI("2", 4f);
        
        Destroy(gameObject);
    }

}
