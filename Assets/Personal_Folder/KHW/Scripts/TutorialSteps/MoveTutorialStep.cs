using System.Collections;
using UnityEngine;
using static TutorialTriggerManager;

public interface TutorialExecution
{
    public abstract void ExecuteTutorial();
}
public abstract class TutorialStepBase : MonoBehaviour, TutorialExecution
{
    public abstract void ExecuteTutorial();
}

public class MoveTutorialStep : TutorialStepBase
{
    public override void ExecuteTutorial()
    {
        TutorialTriggerUIController.Instance.ShowUI("0", 4f);

    }
}
