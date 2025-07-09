using UnityEngine;

public class ChipTutorialStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        TutorialTriggerUIController.Instance.ShowUI("4", 10f);

    }
}
