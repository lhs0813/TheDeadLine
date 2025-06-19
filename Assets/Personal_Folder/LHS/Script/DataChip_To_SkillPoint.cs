using UnityEngine;

public class DataChip_To_SkillPoint : MonoBehaviour
{
    SkillTreeManager _sktm;
    UI_Interact_Info _uiInfo;

    void Start()
    {
        _sktm = FindAnyObjectByType<SkillTreeManager>();
        _uiInfo = FindAnyObjectByType<UI_Interact_Info>();

    }

    public void SkillPointUp()
    {
        _sktm.availablePoints += 1;
        _uiInfo.transform.gameObject.GetComponent<Animator>().SetTrigger("On");
        Destroy(gameObject);
    }


}
