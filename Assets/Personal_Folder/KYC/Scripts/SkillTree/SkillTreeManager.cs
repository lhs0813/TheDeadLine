using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using static SkillNode;

public class SkillTreeManager : MonoBehaviour
{
    public int availablePoints = 0;
    public TextMeshProUGUI pointsText;
    public PlayerStats playerStats;
    public List<SkillNode> allSkills;

    public event Action<int> OnPointChanged;

    private void Start()
    {
        OnPointChanged?.Invoke(availablePoints);
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
        foreach (var skill in allSkills)
        {
            if (skill.IsUnlocked)
            {
                skill.Reset(playerStats);
            }
        }

        availablePoints = allSkills.Count; // 필요 시 수정 가능
        OnPointChanged?.Invoke(availablePoints);
    }

    private void Update()
    {
        if (pointsText != null)
        {
            pointsText.text = $"Available Points: {availablePoints}";
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



}
