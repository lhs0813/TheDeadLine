using UnityEngine;

public class GunThrowStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        TutorialTriggerUIController.Instance.ShowUI("3", 5f);

        Destroy(gameObject);
    }
}
