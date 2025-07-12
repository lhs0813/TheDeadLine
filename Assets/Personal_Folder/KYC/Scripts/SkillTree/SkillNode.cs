using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class SkillNode : MonoBehaviour,  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    float alphaThreshold = 0.1f;
    public string skillId;
    public int requiredPoints = 1;

    public GameObject tooltipTextObject;
    public int currentLevel = 0;
    public int maxLevel = 5;
    public bool IsUnlocked => currentLevel > 0;
    public TextMeshProUGUI tooltipTMP;

    public Button button;
    public Image iconImage;
    public GameObject[] levelDotSets;
    [Header("Infinite Skill Settings")]
    public bool infinite = false;                    // 무한 업그레이드 여부
    public TextMeshProUGUI infiniteLevelText;        // 중앙 숫자 표시용 텍스트

    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        UpdateVisual();
        if (tooltipTextObject != null)
            tooltipTextObject.SetActive(false);
    }

    private void OnClick(PointerEventData eventData)
    {
        SkillTreeManager manager = FindObjectOfType<SkillTreeManager>();

        if (manager != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                manager.TryLevelUpSkill(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                manager.TryLevelDownSkill(this);
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipTextObject != null)
            tooltipTextObject.SetActive(true);
        if (tooltipTextObject != null)
        {
            tooltipTextObject.SetActive(true);
            UpdateTooltipText(); // ← 텍스트 갱신
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipTextObject != null)
            tooltipTextObject.SetActive(false);
    }
    public void LevelUp()
    {
        // infinite 아니라면 기존 maxLevel 체크
        if (!infinite && currentLevel >= maxLevel) return;

        currentLevel++;
        SkillEffectHandler.Instance.ApplyEffectById(skillId, currentLevel);
        UpdateVisual();

        if (infinite)
            UpdateInfiniteText();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(eventData);
    }
    public void Reset()
    {
        if (currentLevel == 0) return;

        SkillEffectHandler.Instance.RemoveEffectById(skillId);
        currentLevel = 0;
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        // 점 세트 처리
        if (levelDotSets != null)
        {
            for (int i = 0; i < levelDotSets.Length; i++)
            {
                if (levelDotSets[i] != null)
                    levelDotSets[i].SetActive(i == currentLevel);
            }
        }
    }
    public void LevelDown()
    {
        if (currentLevel <= 0) return;

        currentLevel--;
        SkillEffectHandler.Instance.ApplyEffectById(skillId, currentLevel);
        UpdateVisual();

        if (infinite)
            UpdateInfiniteText();
    }
    public void UpdateTooltipText()
    {
        if (tooltipTMP == null) return;

        string key = $"SKILL_DESC_{skillId}_{currentLevel}";
        TableReference table = "SkillTable"; // ← 여기!

        LocalizationSettings.StringDatabase
            .GetLocalizedStringAsync(table, key)
            .Completed += handle =>
            {
                tooltipTMP.text = handle.Result;
            };
    }

    private void UpdateInfiniteText()
    {
        if (infiniteLevelText == null) return;

        switch (skillId)
        {
            case "INFINITE_MAX_HEALTH":
                // 1레벨당 체력 +5 → 4레벨이면 4 * 5 = 20
                infiniteLevelText.text = "+" + (150 + currentLevel * 5).ToString();
                break;

            case "INFINITE_BASE_DAMAGE":
                // 1레벨당 데미지 +1% → 5레벨이면 "5%"
                infiniteLevelText.text = "+" + (currentLevel+50).ToString() + "%";
                break;

            default:
                // 그 외 기본 표시
                infiniteLevelText.text = currentLevel.ToString();
                break;
        }
    }
    public void OnInfiniteLevelUp()
    {
        var manager = FindObjectOfType<SkillTreeManager>();
        if (manager != null)
            manager.TryLevelUpSkill(this);  // 포인트 차감 → LevelUp → UI 갱신
    }

    public void OnInfiniteLevelDown()
    {
        var manager = FindObjectOfType<SkillTreeManager>();
        if (manager != null)
            manager.TryLevelDownSkill(this);  // 포인트 반환 → LevelDown → UI 갱신
    }

}
