using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class UI_Objective_Info : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI objectiveText; //목표 표출 텍스트.
    [SerializeField] LocalizedString findFuseLocalizedString;

    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ReturnToTheTrainObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction += DisableText;

        DisableText();
    }

    public void ShowFuseFindingObjective(int counter)
    {
        objectiveText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Objective Table", "Objective_Find Fuse") + $" : {counter} / 3";
    }
    public void ReturnToTheTrainObjective()
    {
        objectiveText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Objective Table", "Objective_Return To The Train");
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
