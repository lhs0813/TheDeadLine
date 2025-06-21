using UnityEngine;
using UnityEngine.AI;

public class ChasingHordeSpawner : MonoBehaviour
{
    public void TrySpawn(int mapIndex, bool danger)
    {
        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(mapIndex)
            .GetRandom(new DunGen.RandomStream());

        if (danger)
        {
            spawnCount *= 3;
        }

        for (int i = 0; i < spawnCount; i++)
            {
                EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);

                //스폰
                GameObject enemy = EnemyPoolManager
                    .Instance
                    .Spawn(type, transform.position, Quaternion.identity, false);
            }
    }
}
