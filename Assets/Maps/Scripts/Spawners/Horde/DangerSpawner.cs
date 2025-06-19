using UnityEngine;

public class DangerSpawner : MonoBehaviour
{
    public void TrySpawn(int mapIndex)
    {
        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(mapIndex)
            .GetRandom(new DunGen.RandomStream());

        for (int i = 0; i < spawnCount; i++)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);

            GameObject enemy = EnemyPoolManager
                .Instance
                .Spawn(type, transform.position, transform.rotation, false); //prespawn이 아니므로, false.
        }
    }
}
