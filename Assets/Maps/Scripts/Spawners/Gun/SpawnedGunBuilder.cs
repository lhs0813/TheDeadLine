using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum WeaponGrade { Normal, Rare, Epic, Legendary }

/// <summary>
/// 무기 등급별 확률 계산 및 프리팹 관리 최적화 버전
/// </summary>
public static class SpawnedGunBuilder
{
    // 미리 할당해둘 등급 배열
    private static readonly WeaponGrade[] Grades = (WeaponGrade[])Enum.GetValues(typeof(WeaponGrade));

    // 스테이지별 확률 캐시 (중복 계산 방지)
    private static readonly Dictionary<int, Dictionary<WeaponGrade, float>> ProbabilityCache = new();

    // 등급별 프리팹 캐시 (Addressables 로드 1회)
    private static readonly Dictionary<WeaponGrade, List<GameObject>> PrefabCache = new();

    /// <summary>
    /// 게임 시작 시 한 번만 호출해 Addressables 로드
    /// </summary>
    public static async Task InitializeAsync()
    {
        foreach (var grade in Grades)
        {
            string label = $"Pickable_Gun_{grade}";
            var handle = Addressables.LoadAssetsAsync<GameObject>(label, null);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                PrefabCache[grade] = handle.Result.ToList();
            else
                PrefabCache[grade] = new List<GameObject>();
        }
    }

    /// <summary>
    /// 등급별 가중치 계산 (원래 로직 유지)
    /// </summary>
    private static float GetWeight(WeaponGrade grade, int stageIndex)
    {
        return grade switch
        {
            WeaponGrade.Normal    => Mathf.Max(100 - stageIndex * 2f, 5),
            WeaponGrade.Rare      => Mathf.Clamp(stageIndex * 1.5f, 5, 40),
            WeaponGrade.Epic      => Mathf.Clamp(stageIndex - 10, 0, 30),
            WeaponGrade.Legendary => Mathf.Clamp((stageIndex - 20) * 0.8f, 0, 25),
            _                     => 0
        };
    }

    /// <summary>
    /// 스테이지별 등급 확률 반환 (백분율, 합 = 100)
    /// </summary>
    public static Dictionary<WeaponGrade, float> GetGradeProbabilities(int stageIndex)
    {
        // 캐시에 있으면 리턴
        if (ProbabilityCache.TryGetValue(stageIndex, out var cached))
            return cached;

        // 새로 계산
        var weights = new Dictionary<WeaponGrade, float>(Grades.Length);
        float total = 0f;
        foreach (var grade in Grades)
        {
            float w = GetWeight(grade, stageIndex);
            weights[grade] = w;
            total += w;
        }

        var probs = new Dictionary<WeaponGrade, float>(Grades.Length);
        foreach (var kv in weights)
            probs[kv.Key] = (kv.Value / total) * 100f;

        ProbabilityCache[stageIndex] = probs;
        return probs;
    }

    /// <summary>
    /// 스테이지 확률에 따라 등급 추첨
    /// </summary>
    public static WeaponGrade RollGrade(int stageIndex)
    {
        var probs = GetGradeProbabilities(stageIndex);
        float roll = UnityEngine.Random.value * 100f;
        float cumulative = 0f;
        foreach (var grade in Grades)
        {
            cumulative += probs[grade];
            if (roll <= cumulative)
                return grade;
        }
        return WeaponGrade.Normal;
    }

    /// <summary>
    /// 미리 로드된 프리팹에서 등급별 랜덤 총기 선택
    /// </summary>
    public static GameObject GetRandomGunPrefab(int stageIndex)
    {
        WeaponGrade grade = RollGrade(stageIndex);

        if (!PrefabCache.TryGetValue(grade, out var list) || list.Count == 0)
        {
            Debug.LogError($"[SpawnedGunBuilder] Prefabs for grade {grade} not loaded or empty.");
            return null;
        }

        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }
}
