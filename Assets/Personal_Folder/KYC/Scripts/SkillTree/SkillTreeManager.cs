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
