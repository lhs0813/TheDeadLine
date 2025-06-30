using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class UI_Electricity_Info : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] LocalizeStringEvent objectiveEvent; 

    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingUI;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ShowFuseFindingEndUI;
    }

    void ShowFuseFindingUI(int count, int max)
    {
        if (count != 0)
        {
            objectiveEvent.StringReference.TableEntryReference = "InteractInfo_Info_Fuse Found";
            objectiveEvent.StringReference.Arguments = new object[]{ max - count };
            // 2) 즉시 갱신
            objectiveEvent.RefreshString();

            FindAnyObjectByType<UI_Electricity_Info>().GetComponent<Animator>().SetTrigger("On");            
        }

    }

    void ShowFuseFindingEndUI()
    {
        objectiveEvent.StringReference.TableEntryReference = "InteractInfo_Info_All Fuse Found";

        objectiveEvent.RefreshString();

        FindAnyObjectByType<UI_Electricity_Info>().GetComponent<Animator>().SetTrigger("On");
    }

    void OnDisable()
    {
        ObjectiveManager.instance.OnFindFuseAction -= ShowFuseFindingUI;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= ShowFuseFindingEndUI;        
    }
}
