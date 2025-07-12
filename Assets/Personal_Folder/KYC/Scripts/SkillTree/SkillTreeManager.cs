using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using static SkillNode;
using Lolopupka;
using Akila.FPSFramework;
using System.Linq;

public class SkillTreeManager : MonoBehaviour
{
    [Header("최종 스킬 언락 후 토글할 UI")]
    [SerializeField] private List<GameObject> uiToDisable;          // 기존에 꺼야 할 7개 오브젝트
    [SerializeField] private List<GameObject> newSkillObjects;      // 새로 보여줄 스킬 2개
    [SerializeField] private List<GameObject> newUIObjects;         // 새로 보여줄 UI 3개
                                                                    // 중복 실행 방지 플래그
    private bool hasUnlockedFinalSkills = false;

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
        CheckAllSkillsMaxed();
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

    private void CheckAllSkillsMaxed()
    {
        // 이미 한 번 실행됐으면 아무것도 안 함
        if (hasUnlockedFinalSkills)
            return;
        // 씬에 있는 모든 SkillNode를 가져와서
        var allNodes = FindObjectsOfType<SkillNode>();

        // 하나라도 maxLevel(5) 미만인 게 있으면 리턴
        if (allNodes.Any(n => n.currentLevel < n.maxLevel))
            return;

        // 전부 다 5레벨 달성했을 때
        // 1) 기존 UI 비활성화
        foreach (var go in uiToDisable)
            go.SetActive(false);

        // 2) 새 스킬 오브젝트 활성화
        foreach (var go in newSkillObjects)
            go.SetActive(true);

        // 3) 새 UI 오브젝트 활성화
        foreach (var go in newUIObjects)
            go.SetActive(true);

        // 한 번 실행 플래그 세팅
        hasUnlockedFinalSkills = true;
        // (원한다면, 이 이벤트는 한 번만 실행되도록
        //  CheckAllSkillsMaxed 자체를 disable 하거나,
        //  bool 플래그를 두고 중복 호출을 막아주세요.)
    }

}
