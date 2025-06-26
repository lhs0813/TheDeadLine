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
    public List<SkillNode> allSkills;
    private float _resetKeyTime = 0f;
    public int resetTicket = 1; // (초기값 원하는 대로)
    public event Action<int> OnPointChanged;

    public RotateOnTrigger laptopTrigger;

    public AudioSource levelUpSounds;
    public AudioSource levelDownSounds;




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

    public bool TryLevelUpSkill(SkillNode skill)
    {
        if (availablePoints < skill.requiredPoints)
            return false;

        if (skill == null || skill.currentLevel >= skill.maxLevel)
            return false;

        skill.LevelUp();
        availablePoints -= skill.requiredPoints;
        OnPointChanged?.Invoke(availablePoints);

        levelUpSounds.Play();
        // 🟡 설명 텍스트도 갱신
        skill.UpdateTooltipText();
        return true;
    }

    public void ResetAllSkills()
    {
        SkillEffectHandler.Instance.ResetAllEffects(); // ✅ 전역 수치도 초기화

        int refundedPoints = 0;
        foreach (var skill in allSkills)
        {
            if (skill.IsUnlocked)
            {
                refundedPoints += skill.requiredPoints;
                skill.Reset(); // SkillEffectHandler에서 RemoveEffectById 호출됨
            }
        }
        availablePoints += refundedPoints;
        OnPointChanged?.Invoke(availablePoints);
    }



    private void Update()
    {
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
    public bool TryLevelDownSkill(SkillNode skill)
    {
        if (skill == null || skill.currentLevel <= 0)
            return false;

        skill.LevelDown();
        availablePoints += skill.requiredPoints;
        OnPointChanged?.Invoke(availablePoints);

        levelDownSounds.Play();
        // 🟡 설명 텍스트도 갱신
        skill.UpdateTooltipText();
        return true;
    }
}
