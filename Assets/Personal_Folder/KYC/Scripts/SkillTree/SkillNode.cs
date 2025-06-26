using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class SkillNode : MonoBehaviour,  IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    float alphaThreshold = 0.1f;
    public string skillId;
    public int requiredPoints = 1;

    public GameObject tooltipTextObject;
    public int currentLevel = 0;
    public int maxLevel = 5;
    public bool IsUnlocked => currentLevel > 0;

    public Button button;
    public Image iconImage;
    public GameObject[] levelDotSets;


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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipTextObject != null)
            tooltipTextObject.SetActive(false);
    }
    public void LevelUp()
    {
        if (currentLevel >= maxLevel) return;

        currentLevel++;
        SkillEffectHandler.Instance.ApplyEffectById(skillId, currentLevel);
        UpdateVisual();
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
    }
}
