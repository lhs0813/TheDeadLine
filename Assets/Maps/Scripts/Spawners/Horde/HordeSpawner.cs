using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class HordeSpawner : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float minSpacing = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    //[SerializeField] private EnemyType spawnType;

    public void TrySpawn(int mapIndex)
    {
        int spawnCount = MapGenCalculator.GetCreatureSpawnCountRangePerRoom(mapIndex).GetRandom(new DunGen.RandomStream());

        List<Vector3> validPositions = new List<Vector3>();
        int attempts = 0;
        int maxAttempts = spawnCount * 10;

        while (validPositions.Count < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                bool tooClose = validPositions.Exists(pos => Vector3.Distance(pos, hit.position) < minSpacing);
                if (!tooClose)
                    validPositions.Add(hit.position);
            }
        }

        foreach (var pos in validPositions)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);
            GameObject enemy = EnemyPoolManager.Instance.Spawn(type, pos, Quaternion.identity);
            // TODO : enemy?.GetComponent<EnemyClass>()?.Initialize();
        }
    }


    private int GetSpawnCount(int mapIndex)
    {
        return MapGenCalculator.GetCreatureSpawnCountRangePerRoom(mapIndex).GetRandom(new DunGen.RandomStream());
    }
}
