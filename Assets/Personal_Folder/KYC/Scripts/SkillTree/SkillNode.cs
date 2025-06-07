using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float alphaThreshold = 0.1f;
    public string skillId;
    public SkillNode[] prerequisites;
    public int requiredPoints = 1;

    public string skillName;
    public string description;
    public VideoClip previewVideo; // 시연 영상

    public enum SkillCategory { Attack, Defend, Utility }
    public SkillCategory category;
    public int exclusiveGroupId = -1; // 같은 카테고리 내 그룹

    public Button button;
    public Image iconImage;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public bool IsUnlocked { get; private set; }


    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        button.onClick.AddListener(OnClick);
        UpdateVisual();
    }

    private void OnClick()
    {
        SkillTreeManager manager = FindObjectOfType<SkillTreeManager>();
        if (manager != null)
        {
            manager.TryUnlockSkill(this);
        }
    }

    public void Unlock()
    {
        if (IsUnlocked) return;

        IsUnlocked = true;
        SkillEffectHandler.Instance.ApplyEffectById(skillId);
        UpdateVisual();
    }

    public void Reset()
    {
        if (!IsUnlocked) return;

        SkillEffectHandler.Instance.RemoveEffectById(skillId);
        IsUnlocked = false;
        UpdateVisual();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillTooltip.Instance.Show(skillName, description, requiredPoints, previewVideo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillTooltip.Instance.Hide();
    }
    public bool ArePrerequisitesMet()
    {
        if (prerequisites == null || prerequisites.Length == 0)
            return true;

        foreach (var node in prerequisites)
        {
            if (node != null && node.IsUnlocked)
                return true;
        }
        return false;
    }


    private void UpdateVisual()
    {
        if (iconImage == null) return;

        if (IsUnlocked)
            iconImage.sprite = unlockedSprite;
        else
            iconImage.sprite = lockedSprite;
    }
}
