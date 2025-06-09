using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum WeaponGrade
{
    Normal,
    Rare,
    Epic,
    Legendary
}

public static class SpawnedGunBuilder
{
    /// <summary>
    /// 원하는 weapongrade의 해당 스테이지에서의 확률을 얻습니다.
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="stageIndex"></param>
    /// <returns></returns>
    private static float GetWeight(WeaponGrade grade, int stageIndex)
    {
        switch (grade)
        {
            // case WeaponGrade.Normal:
            //     return Mathf.Max(100 - stageIndex * 2f, 5); // 점점 줄어듦
            // case WeaponGrade.Rare:
            //     return Mathf.Clamp(stageIndex * 1.5f, 5, 40);
            // case WeaponGrade.Epic:
            //     return Mathf.Clamp(stageIndex - 10, 0, 30);
            // case WeaponGrade.Legendary:
            //     return Mathf.Clamp((stageIndex - 20) * 0.8f, 0, 25);
            // default:
            //     return 0;
            case WeaponGrade.Normal:
                return 0.25f; // 점점 줄어듦
            case WeaponGrade.Rare:
                return 0.25f;
            case WeaponGrade.Epic:
                return 0.25f;
            case WeaponGrade.Legendary:
                return 0.25f;
            default:
                return 0;
        }
    }

    /// <summary>
    /// 등급과 해당 등급의 현재스테이지에서의 확률을 Dictionary형으로 얻습니다.
    /// </summary>
    /// <param name="stageIndex"></param>
    /// <returns></returns>
    public static Dictionary<WeaponGrade, float> GetGradeProbabilities(int stageIndex)
    {
        var weights = new Dictionary<WeaponGrade, float>();
        float total = 0f;

        foreach (WeaponGrade grade in System.Enum.GetValues(typeof(WeaponGrade)))
        {
            float w = GetWeight(grade, stageIndex);
            weights[grade] = w;
            total += w;
        }

        // Normalize
        foreach (var key in weights.Keys.ToList())
        {
            weights[key] = (weights[key] / total) * 100f;
        }


        return weights;
    }

    /// <summary>
    /// 등급과 해당스테이지에서의 등급확률을 Debug 출력합니다.
    /// </summary>
    /// <param name="stageIndex"></param>
    public static void PrintGradeProbabilities(int stageIndex)
    {
        var probs = GetGradeProbabilities(stageIndex);
        foreach (var p in probs)
            Debug.Log($"{p.Key}: {p.Value:F1}%");
    }

    /// <summary>
    /// 해당 스테이지에서의 무기등급확률에 대해 등급을 획득합니다.
    /// </summary>
    /// <param name="stageIndex"></param>
    /// <returns></returns>
    public static WeaponGrade RollGrade(int stageIndex)
    {
        var probs = GetGradeProbabilities(stageIndex);
        float roll = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var pair in probs)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        return WeaponGrade.Normal;
    }
    
    /// <summary>
    /// 해당 스테이지 확률에 기반한 랜덤 총기 프롭 생성.
    /// </summary>
    /// <param name="stageIndex"></param>
    /// <returns></returns>
    public static async Task<GameObject> GetRandomGunPrefab(int stageIndex)
    {
        WeaponGrade grade = RollGrade(stageIndex);
        string label = $"Pickable_Gun_{grade}";

        List<GameObject> loadedPrefabs = new();
        var handle = Addressables.LoadAssetsAsync<GameObject>(label, null);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result.Count == 0)
        {
            Debug.LogError($"라벨이 {label}인 객체 없음.");
            return null;
        }

        loadedPrefabs.AddRange(handle.Result);
        GameObject chosen = loadedPrefabs[Random.Range(0, loadedPrefabs.Count)];
        return chosen;
    }
}
