using UnityEngine;
using UnityEngine.AI;

public class HordeSpawner : MonoBehaviour
{
    // 더 이상 spawnRadius, minSpacing, enemyLayer 등은 필요 없겠죠.
    public void TrySpawn(int mapIndex)
    {
        Debug.Log("스폰 시도");

        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerRoom(mapIndex)
            .GetRandom(new DunGen.RandomStream());

        for (int i = 0; i < spawnCount; i++)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);

            GameObject enemy = EnemyPoolManager
                .Instance
                .Spawn(type, transform, false);
        }
    }
}
