 using UnityEngine;

public class RunTutorialStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        TutorialTriggerUIController.Instance.ShowUI("7", 10f);

        Destroy(gameObject);
    }
}
