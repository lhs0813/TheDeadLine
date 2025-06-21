using System.Collections.Generic;
using UnityEngine;

public static class HordeSpawnBuilder
{
    public static Dictionary<EnemyType, float> GetSpawnWeights(int stageIndex)
    {
        return new Dictionary<EnemyType, float>
        {
            { EnemyType.Normal, Mathf.Max(100 - stageIndex * 3f, 30f) },
            { EnemyType.Big, Mathf.Clamp(stageIndex * 1.5f, 5f, 30f) },
            { EnemyType.Bomb, Mathf.Clamp((stageIndex - 5) * 1.2f, 5f, 20f) }
        };
    }

    public static EnemyType RollEnemyType(int stageIndex)
    {
        var weights = GetSpawnWeights(stageIndex);
        float total = 0;
        foreach (var w in weights.Values)
            total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var pair in weights)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        return EnemyType.Normal; // fallback
    }
}
