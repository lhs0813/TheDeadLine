using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class HordeSpawnBuilder
{
    public static Dictionary<EnemyType, float> spawnWeights;

    public static void SetSpawnWeights(int stageIndex)
    {
        spawnWeights = GetSpawnWeights(stageIndex);
    }

    public static Dictionary<EnemyType, float> GetSpawnWeights(int stageIndex)
    {
        // 1) 원시 가중치 계산 (계수는 9스테이지 목표값 반영)
        float rawNormal = stageIndex > 0
            ? Mathf.Max(100f - 13.028f * Mathf.Log(stageIndex + 1f), 0f)
            : 100f;

        float rawBig = 0f;
        if (stageIndex >= 1)
            rawBig = Mathf.Clamp(4.551f * Mathf.Log(stageIndex), 0f, 10f);

        float rawFast = 0f;
        if (stageIndex >= 3)
            rawFast = Mathf.Clamp(7.709f * Mathf.Log(stageIndex - 2f), 0f, 15f);

        float rawBomb = 0f;
        if (stageIndex >= 4)
            rawBomb = Mathf.Clamp(1.674f * Mathf.Log(stageIndex - 3f), 0f, 3f);

        // 2) 정규화
        float total = rawNormal + rawBig + rawFast + rawBomb;
        if (total > 0f)
        {
            rawNormal = rawNormal / total * 100f;
            rawBig    = rawBig    / total * 100f;
            rawFast   = rawFast   / total * 100f;
            rawBomb   = rawBomb   / total * 100f;
        }

        return new Dictionary<EnemyType, float>
        {
            { EnemyType.Normal, rawNormal },
            { EnemyType.Big,    rawBig    },
            { EnemyType.Fast,   rawFast   },
            { EnemyType.Bomb,   rawBomb   }
        };
    }




    public static EnemyType RollEnemyType(int stageIndex)
    {

        float total = 0;
        foreach (var w in spawnWeights.Values)
            total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var pair in spawnWeights)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        return EnemyType.Normal; // fallback
    }

        /// <summary>
    /// 스테이지별 생성 가중치를 각각 한 줄씩 포맷팅해서 반환합니다.
    /// 예) "Normal: 60.00%\nBig: 20.00%\nFast: 15.00%\nBomb: 5.00%"
    /// </summary>
    public static string GetSpawnWeightsText(int stageIndex)
    {
        // stageIndex에 맞춰 내부 spawnWeights를 업데이트
        SetSpawnWeights(stageIndex);

        var sb = new StringBuilder();
        // 출력 순서를 고정하고 싶다면 아래 배열 순서대로 사용
        var order = new[] { EnemyType.Normal, EnemyType.Big, EnemyType.Fast, EnemyType.Bomb };

        foreach (var type in order)
        {
            float w = spawnWeights.TryGetValue(type, out var weight) ? weight : 0f;
            sb.AppendLine($"{type}: {w:F2}%");
        }

        // 마지막 줄바꿈(\r,\n) 제거
        return sb.ToString().TrimEnd('\r', '\n');
    }
}
