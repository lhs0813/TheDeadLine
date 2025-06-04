using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class HordeSpawner : MonoBehaviour
{
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float minSpacing = 1.5f; 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private EnemyType spawnType;

    public void TrySpawn()
    {
        List<Vector3> validPositions = new List<Vector3>();
        int attempts = 0;
        int maxAttempts = spawnCount * 10;

        while (validPositions.Count < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // NavMesh 위의 위치 찾기
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                bool tooClose = false;
                foreach (var pos in validPositions)
                {
                    if (Vector3.Distance(pos, hit.position) < minSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    validPositions.Add(hit.position);
                }
            }
        }

        foreach (var pos in validPositions)
        {
            GameObject enemy = EnemyPoolManager.Instance.Spawn(spawnType, pos, Quaternion.identity);

            //TODO : enemy.GetComponent<EnemyClass>().Initialize();
            
        }
    }
}
