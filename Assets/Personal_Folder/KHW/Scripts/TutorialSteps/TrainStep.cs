using UnityEngine;

public class TrainStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        TutorialTriggerUIController.Instance.ShowUI("11", 10f);

        Destroy(gameObject);
    }
}
