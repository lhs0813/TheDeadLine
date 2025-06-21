using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectHandler : MonoBehaviour
{
    public static SkillEffectHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        RegisterEffects(); // 딕셔너리 구성
    }

    // 🧠 전역 수치 (외부에서 참조)
    public float headshotDamageMultiplier = 1f; // 헤드샷 데미지 배수
    public float criticalChance = 0f;      // 10% 확률로
    public float criticalMultiplier = 2f;  // 두배 데미지
    public float attackSpeedBonus = 1f; // 공격속도증가
    public float recoilMultiplier = 1f; // 반동감소
    public bool isAmmoInfinite = false; // 무한 탄약 여부
    public bool isHeartofBerserkeravailable = false; // Berserker 효과 활성화 여부
    public float berserkerDamageMultiplier = 1.5f; // Berserker 데미지 배수 (예: 1.5f는 50% 증가)
    public float damageReduction = 1f; // 데미지 감소 배수 (예: 0.8f는 20% 감소)
    public float evasionChance = 0f; // 회피 확률 (예: 0.1f는 10% 확률로 회피)
    public bool isInvinciblePerStation = false; // 이 스킬이 적용 중인지 여부
    public bool absorbHeatlh = false; // 체력 흡수 여부
    public float absorbHeatlhAmount = 1f;
    public bool maxHealthIncrease = false; // 최대 체력 증가 여부
    public float maxHealthIncreaseAmount = 0f; // 최대 체력 증가량 (예: 50 체력 증가)
    public bool isFullHpDamageBoost = false; // 체력 풀일 때 데미지 증가 여부
    public float fullHpDamageMultiplier = 1.0f; 

    // ... 필요에 따라 추가

    // 내부 딕셔너리
    private Dictionary<string, Action<int>> _applyLevelEffects = new();
    private Dictionary<string, Action> _removeEffects = new();


    // ✨ 딕셔너리에 등록
    private void RegisterEffects()
    {
        _applyLevelEffects["HEADSHOT_DAMAGE"] = (level) => headshotDamageMultiplier = 1f + 0.1f * level; // 1.1x ~ 1.5x
        _removeEffects["HEADSHOT_DAMAGE"] = () => headshotDamageMultiplier = 1f;

        _applyLevelEffects["CRIT_CHANCE"] = (level) =>
        {
            criticalChance = 0f + 0.1f * level;
            criticalMultiplier = 2f;
        };
        _removeEffects["CRIT_CHANCE"] = () =>
        {
            criticalChance = 0f;
            criticalMultiplier = 2f;
        };

        _applyLevelEffects["ATTACKSPEED"] = (level) =>
        {
            float[] bonus = { 1f, 0.8333f, 0.7143f, 0.6250f, 0.5556f, 0.5f };
            attackSpeedBonus = bonus[Mathf.Clamp(level, 1, 5)];
        };
        _removeEffects["ATTACKSPEED"] = () => attackSpeedBonus = 1f;

        _applyLevelEffects["RECOIL_REDUCE"] = (level) =>
        {
            float[] recoilbonus = { 1f, 0.9f, 0.7f, 0.5f, 0.3f, 0.1f };
            recoilMultiplier = recoilbonus[Mathf.Clamp(level, 1, 5)];
        };
        _removeEffects["RECOIL_REDUCE"] = () => recoilMultiplier = 1f;



        _applyLevelEffects["HEART_OF_BERSERKER"] = (level) =>
        {
            float[] bonusTable = { 0f, 0.03f, 0.05f, 0.07f, 0.09f, 0.15f };
            berserkerDamageMultiplier = bonusTable[Mathf.Clamp(level, 1, 5)];
            isHeartofBerserkeravailable = true; // Berserker 효과 활성화
        };
        _removeEffects["HEART_OF_BERSERKER"] = () =>
        {
            berserkerDamageMultiplier = 0.0f;
            isHeartofBerserkeravailable = false; // Berserker 효과 비활성화
        };


        _applyLevelEffects["FULLHP_DAMAGE"] = (level) =>
        {
            isFullHpDamageBoost = true;
            fullHpDamageMultiplier = 1.0f + 0.2f * level;
        };

        _removeEffects["FULLHP_DAMAGE"] = () =>
        {
            isFullHpDamageBoost = false;
            fullHpDamageMultiplier = 1f;
        };

        _applyLevelEffects["INFINITE_AMMO"] = (level) => isAmmoInfinite = true; // 무한 탄약
        _removeEffects["INFINITE_AMMO"] = () => isAmmoInfinite = false; // 무한 탄약 해제


        _applyLevelEffects["DAMAGE_REDUCTION"] = (level) => damageReduction = 1f - 0.1f * level; //  데미지 감소
        _removeEffects["DAMAGE_REDUCTION"] = () => damageReduction = 1f; // 원상 복구

        _applyLevelEffects["EVASION"] = (level) => evasionChance = 0f + 0.1f * level; // 20% 회피 확률
        _removeEffects["EVASION"] = () => evasionChance = 0f; // 회피 확률 초기화

        //_applyEffects["STATION_INVINCIBLE_ONCE"] = () => isInvinciblePerStation = true;
        //_removeEffects["STATION_INVINCIBLE_ONCE"] = () => isInvinciblePerStation = false;

        _applyLevelEffects["ABSORB_HEALTH"] = (level) =>
        {
            absorbHeatlh = true; // 체력 흡수 활성화
            absorbHeatlhAmount = 0f + 1f * level; // 레벨에 따라 흡수량 증가 (예: 1, 1.5, 2.0, ...)
        };
        _removeEffects["ABSORB_HEALTH"] = () =>
        {
            absorbHeatlh = false; // 체력 흡수 비활성화
            absorbHeatlhAmount = 0f;
        };

        _applyLevelEffects["MAX_HEALTH_INCREASE"] = (level) =>
        {
            maxHealthIncrease = true; // 최대 체력 증가 활성화
            maxHealthIncreaseAmount = 50f + 50f * level; // 레벨에 따라 최대 체력 증가량 증가 (예: 50, 60, 70, ...)
        };
        _removeEffects["MAX_HEALTH_INCREASE"] = () =>
        {
            maxHealthIncrease = false; // 최대 체력 증가 비활성화
            maxHealthIncreaseAmount = 0f; // 최대 체력 증가량 초기화
        };



        // 🎯 여기다 계속 추가 가능
    }

    public void ApplyEffectById(string skillId, int level)
    {
        if (_applyLevelEffects.TryGetValue(skillId, out var action))
        {
            action.Invoke(level);
            Debug.Log($"✅ 적용됨: {skillId} 레벨 {level}");
        }
        else
        {
            Debug.LogWarning($"❗ 적용 실패: {skillId}는 등록되지 않음");
        }
    }


    public void RemoveEffectById(string skillId)
    {
        if (_removeEffects.TryGetValue(skillId, out var action))
        {
            action.Invoke();
            Debug.Log($"⛔ 제거됨: {skillId}");
        }
        else
        {
            Debug.LogWarning($"❗ 제거 실패: {skillId}는 등록되지 않음");
        }
    }
    public void ResetAllEffects()
    {
     headshotDamageMultiplier = 1f; // 헤드샷 데미지 배수
     criticalChance = 0f;      // 10% 확률로
     criticalMultiplier = 2f;  // 두배 데미지
     attackSpeedBonus = 1f; // 공격속도증가
     recoilMultiplier = 1f; // 반동감소
     isAmmoInfinite = false; // 무한 탄약 여부
     isHeartofBerserkeravailable = false; // Berserker 효과 활성화 여부
     berserkerDamageMultiplier = 1.5f; // Berserker 데미지 배수 (예: 1.5f는 50% 증가)
     damageReduction = 1f; // 데미지 감소 배수 (예: 0.8f는 20% 감소)
     evasionChance = 0f; // 회피 확률 (예: 0.1f는 10% 확률로 회피)
     isInvinciblePerStation = false; // 이 스킬이 적용 중인지 여부
     absorbHeatlh = false; // 체력 흡수 여부
     absorbHeatlhAmount = 1f;
     maxHealthIncrease = false; // 최대 체력 증가 여부
     maxHealthIncreaseAmount = 50f; // 최대 체력 증가량 (예: 50 체력 증가)
     isFullHpDamageBoost = false; // 체력 풀일 때 데미지 증가 여부
     fullHpDamageMultiplier = 1.0f;
    Debug.Log("[SkillEffectHandler] 모든 스킬 효과 초기화됨");
    }
}
