using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrespawnedHordeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField, Tooltip("반경 내 NavMesh 상의 랜덤 지점에서 스폰")]
    private float spawnRadius = 10f;
    [SerializeField, Tooltip("스폰 지점 간 최소 거리")]
    private float minSpawnDistance = 5f;

    private readonly List<GameObject> preSpawnedEnemies = new List<GameObject>();

    private List<Vector3> spawnPoints;

    public void InitializeSpawnPoints()
    {
        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(GamePlayManager.instance.currentMapIndex)
            .GetRandom(new DunGen.RandomStream());

        spawnPoints = GetNonOverlappingNavMeshPoints(
            center: transform.position,
            count: spawnCount,
            radius: spawnRadius,
            minDistance: minSpawnDistance
        );

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("[HordeSpawner] 유효 스폰 포인트를 하나도 찾지 못했습니다!");
            return;
        }
    }

    /// <summary>
    /// 동시 스폰: 미리 지점을 뽑아두고, 각 지점에 한번에 적 생성
    /// </summary>
    public void TrySpawn(int mapIndex)
    {
        foreach (var point in spawnPoints)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);

            //Transform 설정
            float randomY = Random.Range(0f, 360f);
            GameObject enemy = EnemyPoolManager.Instance.Spawn(type, point, Quaternion.Euler(0f, randomY, 0f), true);
            if (enemy != null)
            {
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
        int maxAttempts = count * 100;  // 시도 횟수 늘림

        while (points.Count < count && attempts < maxAttempts)
        {
            attempts++;
            // y 고정 → 높이 변화 없이 평면 내에서만 샘플링
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0f;
            Vector3 candidate = center + randomDir * radius;

            // radius 만큼의 반경까지 샘플링
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
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

        if (points.Count < count)
            Debug.LogWarning($"[HordeSpawner] 원하는 {count}개 중 {points.Count}개만 찾았습니다. " +
                            "minDistance를 줄이거나 radius/maxAttempts를 늘려보세요.");

        return points;
    }

    public void DeSpawn()
    {
        foreach (var enemy in preSpawnedEnemies)
        {
            if (enemy != null)
            {
                var id = enemy.GetComponent<EnemyIdentifier>();
                if (id != null && id.wasTrackingPlayer) //null 아님, 추적하지 않은 적들만.
                {
                    //아무것도 하지 않음.
                }
                else if (id != null && !id.wasTrackingPlayer) //추적을 하지는 않았음. 
                {
                    EnemyPoolManager.Instance.ReturnToPool(id.Type, enemy, 0f);
                }
            }
        }
        preSpawnedEnemies.Clear();
    }
}
