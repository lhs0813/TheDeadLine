using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class ActiveSkillEffectsUI : MonoBehaviour
{
    public SkillEffectHandler skillEffectHandler;

    public GameObject effectTextPrefab;
    public Transform effectsContainer;

    private Dictionary<string, TextMeshProUGUI> effectTexts = new();

    private void Start()
    {
        RefreshSkillEffectsUI();
    }

    public void RefreshSkillEffectsUI()
    {
        ClearUI();

        if (skillEffectHandler.headshotDamageMultiplier > 1f)
            AddEffect("HEADSHOT_DAMAGE", $"{(skillEffectHandler.headshotDamageMultiplier - 1f) * 100:0}%");

        if (skillEffectHandler.magazineIncreaseMultiplier > 1f)
            AddEffect("MAGAZINE_INCREASE", $"{(skillEffectHandler.magazineIncreaseMultiplier - 1f) * 100:0}%");

        if (skillEffectHandler.attackSpeedBonus < 1f)
        {
            float fireRateMultiplier = 1f / skillEffectHandler.attackSpeedBonus;
            float bonusPercent = (fireRateMultiplier - 1f) * 100f;
            AddEffect("ATTACKSPEED", $"+{bonusPercent:0}%");
        }


        if (skillEffectHandler.recoilMultiplier < 1f)
            AddEffect("RECOIL_REDUCE", $"{(1f - skillEffectHandler.recoilMultiplier) * 100:0}%");

        if (skillEffectHandler.absorbHeatlh)
            AddEffect("ABSORB_HEALTH", $"{skillEffectHandler.absorbHeatlhAmount}");

        if (skillEffectHandler.maxHealthIncrease)
            AddEffect("MAX_HEALTH_INCREASE", $"{skillEffectHandler.maxHealthIncreaseAmount}");

        if (skillEffectHandler.damageReduction < 1f)
            AddEffect("DAMAGE_REDUCTION", $"{(1f - skillEffectHandler.damageReduction) * 100:0}%");

        if (skillEffectHandler.evasionChance > 0f)
            AddEffect("EVASION", $"{skillEffectHandler.evasionChance * 100:0}%");

        if (skillEffectHandler.isHeartofBerserkeravailable)
            AddEffect("HEART_OF_BERSERKER", $"{skillEffectHandler.berserkerDamageMultiplier * 100:0}%");

    }

    private void AddEffect(string localizationKey, string effectValue)
    {
        var localizedString = new LocalizedString("SkillEffectTable", localizationKey);

        localizedString.GetLocalizedStringAsync().Completed += handle =>
        {
            string translatedText = handle.Result;

            var textObj = Instantiate(effectTextPrefab, effectsContainer);
            var tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = $"{translatedText}{effectValue}";

            effectTexts.Add(localizationKey, tmp);
        };
    }

    private void ClearUI()
    {
        foreach (Transform child in effectsContainer)
        {
            Destroy(child.gameObject);
        }
        effectTexts.Clear();
    }
}
