using TMPro;
using UnityEngine;

public class UI_Electricity_Info : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText;

    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingUI;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ShowFuseFindingEndUI;
    }

    void ShowFuseFindingUI(int count)
    {
        if (count != 0)
        {
            descriptionText.text = $"{3 - count}개를 더 찾아 열차 활성화";
            FindAnyObjectByType<UI_Electricity_Info>().GetComponent<Animator>().SetTrigger("On");            
        }

    }

    void ShowFuseFindingEndUI()
    {
        descriptionText.text = $"열차에 탑승해 출발";
        FindAnyObjectByType<UI_Electricity_Info>().GetComponent<Animator>().SetTrigger("On");
    }

    void OnDisable()
    {
        ObjectiveManager.instance.OnFindFuseAction -= ShowFuseFindingUI;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= ShowFuseFindingEndUI;        
    }
}
