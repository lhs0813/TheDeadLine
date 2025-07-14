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
    public float attackSpeedBonus = 1f; // 공격속도증가
    public float recoilMultiplier = 1f; // 반동감소
    public bool isHeartofBerserkeravailable = false; // Berserker 효과 활성화 여부
    public float berserkerDamageMultiplier = 1.5f; // Berserker 데미지 배수 (예: 1.5f는 50% 증가)
    public float damageReduction = 1f; // 데미지 감소 배수 (예: 0.8f는 20% 감소)
    public float evasionChance = 0f; // 회피 확률 (예: 0.1f는 10% 확률로 회피)
    public bool isInvinciblePerStation = false; // 이 스킬이 적용 중인지 여부
    public bool absorbHeatlh = false; // 체력 흡수 여부
    public float absorbHeatlhAmount = 1f;
    public bool maxHealthIncrease = false; // 최대 체력 증가 여부
    public float maxHealthIncreaseAmount = 0f; // 최대 체력 증가량 (예: 50 체력 증가)
    public float magazineIncreaseMultiplier = 1f; // 탄창 증가 배수 (예: 1.2f는 20% 증가)
    public float throwHitMore = 0;
    public float bonusDamegeRate = 1f; // 추가 데미지 비율 (예: 0.1f는 10% 추가 데미지)
    public float bonusMoveSpeed = 1f; // 이동 속도 증가 배수 (예: 1.2f는 20% 증가)
    // ... 필요에 따라 추가

    // 내부 딕셔너리
    private Dictionary<string, Action<int>> _applyLevelEffects = new();
    private Dictionary<string, Action> _removeEffects = new();


    // ✨ 딕셔너리에 등록
    private void RegisterEffects()
    {
        _applyLevelEffects["HEADSHOT_DAMAGE"] = (level) => headshotDamageMultiplier = 1f + 0.2f * level; // 1.1x ~ 1.5x
        _removeEffects["HEADSHOT_DAMAGE"] = () => headshotDamageMultiplier = 1f;

        _applyLevelEffects["ATTACKSPEED"] = (level) =>
        {
            float[] bonus = { 1f, 0.8333f, 0.7143f, 0.6250f, 0.5556f, 0.5f };
            attackSpeedBonus = bonus[Mathf.Clamp(level, 1, 5)];
        };
        _removeEffects["ATTACKSPEED"] = () => attackSpeedBonus = 1f;

        _applyLevelEffects["RECOIL_REDUCE"] = (level) =>
        {
            float[] recoilbonus = { 1f, 0.8f, 0.6f, 0.4f, 0.2f, 0.01f };
            recoilMultiplier = recoilbonus[Mathf.Clamp(level, 1, 5)];
        };
        _removeEffects["RECOIL_REDUCE"] = () => recoilMultiplier = 1f;



        _applyLevelEffects["HEART_OF_BERSERKER"] = (level) =>
        {
            float[] bonusTable = { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
            berserkerDamageMultiplier = bonusTable[Mathf.Clamp(level, 1, 5)];
            isHeartofBerserkeravailable = true; // Berserker 효과 활성화
        };
        _removeEffects["HEART_OF_BERSERKER"] = () =>
        {
            berserkerDamageMultiplier = 0.0f;
            isHeartofBerserkeravailable = false; // Berserker 효과 비활성화
        };

        _applyLevelEffects["DAMAGE_REDUCTION"] = (level) => damageReduction = 1f - 0.1f * level; //  데미지 감소
        _removeEffects["DAMAGE_REDUCTION"] = () => damageReduction = 1f; // 원상 복구

        _applyLevelEffects["EVASION"] = (level) => evasionChance = 0f + 0.1f * level; // 20% 회피 확률
        _removeEffects["EVASION"] = () => evasionChance = 0f; // 회피 확률 초기화

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
            maxHealthIncreaseAmount = 30f * level; // 레벨에 따라 최대 체력 증가량 증가
        };
        _removeEffects["MAX_HEALTH_INCREASE"] = () =>
        {
            maxHealthIncrease = false; // 최대 체력 증가 비활성화
            maxHealthIncreaseAmount = 0f; // 최대 체력 증가량 초기화
        };
        _applyLevelEffects["MAGAZINE_INCREASE"] = (level) =>
        {
            magazineIncreaseMultiplier = 1f + 0.2f * level; // 레벨에 따라 탄창 증가 배수 (예: 1.2, 1.4, ...)
        };
        _removeEffects["MAGAZINE_INCREASE"] = () =>
        {
            magazineIncreaseMultiplier = 1f; // 탄창 증가 배수 초기화
        };
        _applyLevelEffects["THROW_HIT_MORE"] = (level) =>
        {
            throwHitMore = level; // 레벨에 따라 던지기 적중 증가 
        };
        _removeEffects["THROW_HIT_MORE"] = () =>
        {
            throwHitMore = 0; // 던지기 적중 초기화
        };
        _applyLevelEffects["BONUS_DAMAGE_RATE"] = (level) =>
        {
            bonusDamegeRate = 1+ 0.1f * level; // 레벨에 따라 추가 데미지 비율 증가 (예: 0.1, 0.2, ...)
        };
        _removeEffects["BONUS_DAMAGE_RATE"] = () =>
        {
            bonusDamegeRate = 1f; // 추가 데미지 비율 초기화
        };
        _applyLevelEffects["BONUS_MOVEMENT_SPEED"] = (level) =>
        {
            bonusMoveSpeed = 1f + 0.1f * level; // 레벨에 따라 이동 속도 증가 배수 (예: 1.1, 1.2, ...)
        };
        _removeEffects["BONUS_MOVEMENT_SPEED"] = () =>
        {
            bonusMoveSpeed = 1f; // 이동 속도 증가 배수 초기화
        };
        _applyLevelEffects["INFINITE_MAX_HEALTH"] = level =>
        {
            maxHealthIncrease = true;
            maxHealthIncreaseAmount = 150 + 5f * level;
        };
        _removeEffects["INFINITE_MAX_HEALTH"] = () =>
        {
            maxHealthIncrease = false;
            maxHealthIncreaseAmount = 150f;
        };

        // ● 무한 스킬: 기본 피해량 +1% per level
        _applyLevelEffects["INFINITE_BASE_DAMAGE"] = level =>
        {
            bonusDamegeRate = 1.5f + 0.01f * level;
        };
        _removeEffects["INFINITE_BASE_DAMAGE"] = () =>
        {
            bonusDamegeRate = 1.5f;
        };
        // 🎯 여기다 계속 추가 가능
    }

    public void ApplyEffectById(string skillId, int level)
    {
        if (_applyLevelEffects.TryGetValue(skillId, out var action))
        {
            action.Invoke(level);
        }
        else
        {
        }
    }


    public void RemoveEffectById(string skillId)
    {
        if (_removeEffects.TryGetValue(skillId, out var action))
        {
            action.Invoke();
        }
        else
        {
        }
    }
}
