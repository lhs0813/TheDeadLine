using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using static SkillNode;
using Lolopupka;
using Akila.FPSFramework;

public class SkillTreeManager : MonoBehaviour
{
    

    private Controls input;

    public int availablePoints = 0;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI pointsText2;
    public PlayerStats playerStats;
    public List<SkillNode> allSkills;
    private float _resetKeyTime = 0f;
    public int resetTicket = 1; // (초기값 원하는 대로)

    public event Action<int> OnPointChanged;

    public RotateOnTrigger laptopTrigger;


    private void Start()
    {
        input = new Controls();
        OnPointChanged?.Invoke(availablePoints);

        input.UI.Pause.performed += ctx =>
        {
            if (laptopTrigger.isLapTopOn)
            {
                Debug.Log("노트북 꺼짐");
                laptopTrigger.LabtopOff();
            }
            
        };

    }

    public bool TryUnlockSkill(SkillNode skill)
    {
        if (availablePoints < skill.requiredPoints)
            return false;

        if (skill == null || skill.IsUnlocked)
            return false;

        // ✅ 같은 그룹에 이미 해금된 스킬이 있다면 실패
        if (skill.exclusiveGroupId != -1 &&
            IsAnyInGroupUnlocked(skill.category, skill.exclusiveGroupId, skill))
        {
            Debug.Log($"스킬 {skill.skillId}는 같은 그룹 내 다른 스킬이 이미 언락되어 있어요.");
            return false;
        }

        // ✅ prerequisite 확인은 그 다음
        if (!skill.ArePrerequisitesMet())
            return false;

        skill.Unlock(playerStats);
        availablePoints -= skill.requiredPoints;
        OnPointChanged?.Invoke(availablePoints);
        return true;
    }


    public void ResetAllSkills()
    {
        int refundedPoints = 0;
        foreach (var skill in allSkills)
        {
            if (skill.IsUnlocked)
            {
                refundedPoints += skill.requiredPoints;
                skill.Reset(playerStats);
            }
        }
        availablePoints += refundedPoints;
        OnPointChanged?.Invoke(availablePoints);
    }


    private void Update()
    {
        if (laptopTrigger.isLapTopOn)
            input.UI.Enable();
        else
            input.UI.Disable();

        if (pointsText != null)
        {
            pointsText.text = $"{availablePoints}";
        }
        if (pointsText2 != null)
        {
            pointsText2.text = $"{availablePoints}";
        }
        // 리셋 키(예: F) 3초 동안 누르기
        if (input.Player.Intract.ReadValue<float>() > 0f)
        {
            _resetKeyTime += Time.unscaledDeltaTime;

            if (_resetKeyTime >= 3f)
            {
                TryResetAllSkills();
                _resetKeyTime = 0f; // 한 번만 실행
            }
        }
        else
        {
            _resetKeyTime = 0f;
        }
    }

    public bool IsAnyInGroupUnlocked(SkillCategory category, int groupId, SkillNode self)
    {
        return allSkills.Exists(skill =>
            skill != self &&                             // 🧨 자신 제외
            skill.category == category &&
            skill.exclusiveGroupId == groupId &&
            skill.IsUnlocked);
    }

    public void TryResetAllSkills()
    {
        if (resetTicket > 0)
        {
            resetTicket--;
            ResetAllSkills();
            Debug.Log("스킬을 전부 초기화했습니다. 남은 리셋권: " + resetTicket);
        }
        else
        {
            Debug.Log("리셋권이 없습니다!");
        }
    }

}
