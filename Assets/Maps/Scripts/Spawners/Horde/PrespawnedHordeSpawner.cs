using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrespawnedHordeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField, Tooltip("반경 내 NavMesh 상의 랜덤 지점에서 스폰")]
    private float spawnRadius = 5f;
    [SerializeField, Tooltip("스폰 지점 간 최소 거리")]
    private float minSpawnDistance = 2f;

    private readonly List<GameObject> preSpawnedEnemies = new List<GameObject>();

    /// <summary>
    /// 동시 스폰: 미리 지점을 뽑아두고, 각 지점에 한번에 적 생성
    /// </summary>
    public void TrySpawn(int mapIndex)
    {
        Debug.Log("스폰 시도");

        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(mapIndex)
            .GetRandom(new DunGen.RandomStream());

        // 1) 겹치지 않는 NavMesh 지점들 뽑기
        var spawnPoints = GetNonOverlappingNavMeshPoints(
            center: transform.position,
            count: spawnCount,
            radius: spawnRadius,
            minDistance: minSpawnDistance
        );

        // 2) 각 지점에 스폰
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);
            GameObject enemy = EnemyPoolManager.Instance
                .Spawn(type, transform, true);

            if (enemy != null)
            {
                enemy.transform.position = spawnPoints[i];
                preSpawnedEnemies.Add(enemy);
            }
        }
    }

    /// <summary>
    /// center 기준 반경 내 NavMesh 유효 지점을 뽑되,
    /// 서로 minDistance 이상 떨어지도록 시도
    /// </summary>
    private List<Vector3> GetNonOverlappingNavMeshPoints(
        Vector3 center,
        int count,
        float radius,
        float minDistance)
    {
        var points = new List<Vector3>();
        int attempts = 0;
        int maxAttempts = count * 30;

        while (points.Count < count && attempts < maxAttempts)
        {
            attempts++;
            Vector3 candidate = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                bool tooClose = false;
                foreach (var p in points)
                {
                    if (Vector3.Distance(p, hit.position) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                    points.Add(hit.position);
            }
        }

        // 부족한 만큼은 스폰러 위치에 채워넣기
        while (points.Count < count)
            points.Add(center);

        return points;
    }

    public void DeSpawn()
    {
        foreach (var enemy in preSpawnedEnemies)
        {
            if (enemy != null)
            {
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
