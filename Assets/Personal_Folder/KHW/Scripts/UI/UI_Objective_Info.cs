using TMPro;
using UnityEngine;

public class UI_Objective_Info : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI objectiveText; //목표 표출 텍스트.

    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ReturnToTheTrainObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction += DisableText;

        DisableText();
    }

    public void ShowFuseFindingObjective(int counter)
    {
        objectiveText.text = $"퓨즈 찾기 : {counter} / 3";
    }

    public void ReturnToTheTrainObjective()
    {
        objectiveText.text = $"열차로 복귀";
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
