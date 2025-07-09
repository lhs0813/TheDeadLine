using TMPro;
using UnityEngine;

public class Objective_Mission : MonoBehaviour
{
    Animator anim;
    public TextMeshProUGUI fuseText;
    private void Start()
    {
        GamePlayManager.instance.OnTrainArriveAction += ObjectiveMission;
        anim = GetComponent<Animator>();
    }

    void ObjectiveMission()
    {
        int fuseCount = GamePlayManager.instance.currentStageInfo.fuseCount;
        fuseText.SetText(fuseText.text, fuseCount);
        anim.SetTrigger("On");
    }
}
