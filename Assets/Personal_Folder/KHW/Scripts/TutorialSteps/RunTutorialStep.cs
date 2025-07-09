 using UnityEngine;

public class RunTutorialStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        TutorialTriggerUIController.Instance.ShowUI("7", 5f);

    }
}
