using TMPro;
using UnityEngine;

public class UI_Objective_Info : MonoBehaviour
{
    TextMeshProUGUI objectiveText; //목표 표출 텍스트.

    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ReturnToTheTrainObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction += DisableText;

        DisableText();
    }

    public void ShowFuseFindingObjective(int counter)
    {
        objectiveText.text = $"Find 3 of the Fuses : {counter} / 3";
    }

    public void ReturnToTheTrainObjective()
    {
        objectiveText.text = $"Return To The Train";
    }

    public void DisableText()
    {
        objectiveText.text = "";
    }

    void OnDisable()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ReturnToTheTrainObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction += DisableText;
    }
}
