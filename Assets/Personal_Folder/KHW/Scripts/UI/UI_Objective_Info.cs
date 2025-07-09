using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;

public class UI_Objective_Info : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent objectiveEvent;   // 하나만 연결
    [SerializeField] TextMeshProUGUI objectiveText;       // Disable용
    [SerializeField] GameObject stationName;
    [SerializeField] GameObject backImg;

    [SerializeField] bool isActive;

    MapUIController map;

    Animator anim;
    void Start()
    {
        ObjectiveManager.instance.OnFindFuseAction += ShowFuseFindingObjective;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += ShowReturnObjective;
        ObjectiveManager.instance.OnDisableAllObjectiveAction += DisableText;

        map = FindAnyObjectByType<MapUIController>();
        anim = GetComponent<Animator>();
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
            anim.SetTrigger("On");
            objectiveText.gameObject.SetActive(true);
            //0709 김용찬 수정 튜토리얼에선 맵 이름 안뜨게 수정 (사유: 조잡함)

            if(SceneManager.GetActiveScene().name == "StoryMode" || SceneManager.GetActiveScene().name == "StoryModeLoop")
            {
                int mapIndex = GamePlayManager.instance.currentMapIndex;
                if (mapIndex != 0)
                {
                    stationName.SetActive(true);
                    stationName.GetComponentInChildren<TextMeshProUGUI>().text = map.centerStationText.text;
                }
                else
                {
                    stationName.SetActive(false);
                }
            }
            else
            {
                int mapIndex = GamePlayManager.instance.currentMapIndex;
                if (mapIndex != 0)
                {
                    stationName.SetActive(true);
                    stationName.GetComponentInChildren<TextMeshProUGUI>().text =  "Station " + (GamePlayManager.instance.currentMapIndex - 4).ToString();
                }
                else
                {
                    stationName.SetActive(false);
                }
            }


            isActive = true;
            backImg.SetActive(true);
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
            anim.SetTrigger("On");
            //stationName.SetActive(true);
            backImg.SetActive(true);
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
        stationName.SetActive(false);
        backImg.SetActive(false);



    }
}
