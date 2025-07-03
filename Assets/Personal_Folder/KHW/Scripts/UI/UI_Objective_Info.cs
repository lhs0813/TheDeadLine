using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class UI_Objective_Info : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent objectiveEvent;   // 하나만 연결
    [SerializeField] TextMeshProUGUI objectiveText;       // Disable용
    [SerializeField] GameObject lineObj;
    [SerializeField] bool isActive;

    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ShowReturnObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction += DisableText;
        
        DisableText();
    }

    void OnDisable()
    {
        ObjectiveManager.instance.OnFindFuseAction -= ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= ShowReturnObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction  -= DisableText;
    }


    void ShowFuseFindingObjective(int count, int max)
    {
        if (!isActive)
        {
            objectiveText.gameObject.SetActive(true);
            lineObj.SetActive(true);
            isActive = true;
        }

        // 1) 키와 인자 바꾸기
        objectiveEvent.StringReference.TableEntryReference = "Objective_Find Fuse";
        objectiveEvent.StringReference.Arguments = new object[] { count, max };
        // 2) 즉시 갱신
        objectiveEvent.RefreshString();
        
        
    }

    void ShowReturnObjective()
    {
        if (!isActive)
        {
            objectiveText.gameObject.SetActive(true);
            lineObj.SetActive(true);
            isActive = true;
        }

        objectiveEvent.StringReference.TableEntryReference = "Objective_Return To The Train";
        objectiveEvent.StringReference.Arguments = null;  // 인자 없으므로 null
        objectiveEvent.RefreshString();
    }

    void DisableText()
    {
        if (isActive)
        {
            isActive = false;
        }

        objectiveText.gameObject.SetActive(false);
        lineObj.SetActive(false);



    }
}
