using UnityEngine;

public class DataChip_To_SkillPoint : MonoBehaviour
{
    SkillTreeManager _sktm;
    void Start()
    {
        _sktm = FindAnyObjectByType<SkillTreeManager>();
    }

    public void SkillPointUp()
    {
        _sktm.availablePoints += 1;
        Destroy(gameObject);
    }
}
