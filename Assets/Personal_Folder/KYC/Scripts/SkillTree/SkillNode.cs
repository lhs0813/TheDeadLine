using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float alphaThreshold = 0.1f;
    public string skillId;
    public int requiredPoints = 1;

    public string nameKey;
    public string descriptionKey;
    public VideoClip previewVideo; // 시연 영상


    public int currentLevel = 0;
    public int maxLevel = 5;
    public bool IsUnlocked => currentLevel > 0;

    public Button button;
    public Image iconImage;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public GameObject[] levelDotSets;


    private void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
        button.onClick.AddListener(OnClick);
        UpdateVisual();
    }

    private void OnClick()
    {
        SkillTreeManager manager = FindObjectOfType<SkillTreeManager>();
       Debug.Log($"스킬 눌림");
        if (manager != null)
        {
            manager.TryLevelUpSkill(this);
        }
    }

    public void LevelUp()
    {
        if (currentLevel >= maxLevel) return;

        currentLevel++;
        SkillEffectHandler.Instance.ApplyEffectById(skillId, currentLevel);
        UpdateVisual();
    }

    public void Reset()
    {
        if (currentLevel == 0) return;

        SkillEffectHandler.Instance.RemoveEffectById(skillId);
        currentLevel = 0;
        UpdateVisual();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillTooltip.Instance.Show(nameKey, descriptionKey, requiredPoints, previewVideo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillTooltip.Instance.Hide();
    }
    private void UpdateVisual()
    {
        if (iconImage != null)
            iconImage.sprite = IsUnlocked ? unlockedSprite : lockedSprite;

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
}
