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
        DontDestroyOnLoad(gameObject);

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
    public float damageReduction = 1f; // 데미지 감소 배수 (예: 0.8f는 20% 감소)
    // ... 필요에 따라 추가

    // 내부 딕셔너리
    private Dictionary<string, Action> _applyEffects = new();
    private Dictionary<string, Action> _removeEffects = new();

    // ✨ 딕셔너리에 등록
    private void RegisterEffects()
    {
        _applyEffects["HEADSHOT_20"] = () => headshotDamageMultiplier = 1.5f;
        _removeEffects["HEADSHOT_20"] = () => headshotDamageMultiplier = 1f;

        _applyEffects["CRIT_10_200"] = () =>
        {
            criticalChance = 0.1f;
            criticalMultiplier = 2f;
        };
        _removeEffects["CRIT_10_200"] = () =>
        {
            criticalChance = 0f;
            criticalMultiplier = 2f;
        };

        _applyEffects["ATTACKSPEED_50"] = () => attackSpeedBonus = 0.65f;
        _removeEffects["ATTACKSPEED_50"] = () => attackSpeedBonus = 1f;

        _applyEffects["RECOIL_REDUCE_80"] = () => recoilMultiplier = 0.2f; // 반동 80% 감소
        _removeEffects["RECOIL_REDUCE_80"] = () => recoilMultiplier = 1f;


        _applyEffects["INFINITE_AMMO"] = () => isAmmoInfinite = true; // 무한 탄약
        _removeEffects["INFINITE_AMMO"] = () => isAmmoInfinite = false; // 무한 탄약 해제

        _applyEffects["HEART_OF_BERSERKER"] = () => isHeartofBerserkeravailable = true; // Berserker 효과 활성화
        _removeEffects["HEART_OF_BERSERKER"] = () => isHeartofBerserkeravailable = false; // Berserker 효과 비활성화

        _applyEffects["DAMAGE_REDUCTION_20"] = () => damageReduction = 0.8f; // 20% 데미지 감소
        _removeEffects["DAMAGE_REDUCTION_20"] = () => damageReduction = 1f; // 원상 복구
        // 🎯 여기다 계속 추가 가능
    }

    public void ApplyEffectById(string skillId)
    {
        if (_applyEffects.TryGetValue(skillId, out var action))
        {
            action.Invoke();
            Debug.Log($"✅ 적용됨: {skillId}");
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
        headshotDamageMultiplier = 1f;
        criticalChance = 0f;
        criticalMultiplier = 1f;
        attackSpeedBonus = 1f;
        isAmmoInfinite = false;
        isHeartofBerserkeravailable = false;
        damageReduction = 1f;
        // 필요 수치 모두 원상 복구
        Debug.Log("[SkillEffectHandler] 모든 스킬 효과 초기화됨");
    }
}
