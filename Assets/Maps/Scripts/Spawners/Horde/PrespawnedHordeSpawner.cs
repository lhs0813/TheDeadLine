using System.Collections.Generic;
using UnityEngine;

public class PrespawnedHordeSpawner : MonoBehaviour
{
    private readonly List<GameObject> preSpawnedEnemies = new List<GameObject>();
    public void TrySpawn(int mapIndex)
    {
        Debug.Log("스폰 시도");

        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(mapIndex)
            .GetRandom(new DunGen.RandomStream());

        for (int i = 0; i < spawnCount; i++)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);

            GameObject enemy = EnemyPoolManager
                .Instance
                .Spawn(type, transform, true); //prespawn이므로, true.
        }
    }

    /// <summary>
    /// 플레이어가 충분히 멀어졌거나, isAlreadyTriggered 플래그 해제 시점에 호출
    /// </summary>
    public void DeSpawn()
    {
        foreach (var enemy in preSpawnedEnemies)
        {
            if (enemy != null)
            {
                // EnemyPoolManager에 맞게 반환
                var id = enemy.GetComponent<EnemyIdentifier>();
                if (id != null)
                    EnemyPoolManager.Instance.ReturnToPool(id.Type, enemy);
                else
                    Destroy(enemy);
            }
        }
        preSpawnedEnemies.Clear();
    }

}
