using System.Collections;
using System.Collections.Generic;
using DunGen;
using UnityEngine;
using UnityEngine.AI;

public class MainPathSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField, Tooltip("반경 내 NavMesh 상의 랜덤 지점에서 스폰")]
    private float spawnRadius = 10f;
    [SerializeField, Tooltip("스폰 지점 간 최소 거리")]
    private float minSpawnDistance = 5f;

    private readonly List<GameObject> preSpawnedEnemies = new List<GameObject>();
    private List<Vector3> spawnPoints;

    // int → float로 변경
    private float dangerSpawnMultiplier = 1.5f;

    public void InitializeSpawnPoints(int mapIndex)
    {   
        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(mapIndex)
            .GetRandom(new DunGen.RandomStream());

        spawnPoints = GetNonOverlappingNavMeshPoints(
            center: transform.position,
            count: spawnCount,
            radius: spawnRadius,
            minDistance: minSpawnDistance
        );

        if (spawnPoints.Count == 0)
        {
            Debug.LogError($"[HordeSpawner] 유효 스폰 포인트를 하나도 찾지 못했습니다! {GetComponentInParent<Tile>().gameObject.name}");
        }
    }

    /// <summary>
    /// 분할 스폰: 한 프레임에 한 번씩
    /// </summary>
    public void MainSpawn(int mapIndex, bool track, bool danger)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(SpawnRoutine(mapIndex, track, danger));
    }

    private IEnumerator SpawnRoutine(int mapIndex, bool track, bool danger)
    {
        // multiplier 계산
        float multiplier = danger ? dangerSpawnMultiplier : 1f;
        int guaranteed = Mathf.FloorToInt(multiplier);   // 무조건 스폰 수
        float fractional = multiplier - guaranteed;      // 확률 스폰 부분

        foreach (var point in spawnPoints)
        {
            // 1) guaranteed 만큼 스폰
            for (int i = 0; i < guaranteed; i++)
                SpawnOne(mapIndex, track, point);

            // 2) fractional 확률로 한 마리 추가
            if (danger && Random.value < fractional)
                SpawnOne(mapIndex, track, point);

            yield return null;
        }
    }

    private void SpawnOne(int mapIndex, bool track, Vector3 point)
    {
        float randomY = Random.Range(0f, 360f);
        EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);
        GameObject enemy = EnemyPoolManager
            .Instance
            .Spawn(type, point, Quaternion.Euler(0f, randomY, 0f), !track);

        if (enemy != null)
            preSpawnedEnemies.Add(enemy);
    }

    // 이하 GetNonOverlappingNavMeshPoints, DeSpawn 등은 그대로 유지
    private List<Vector3> GetNonOverlappingNavMeshPoints(
        Vector3 center,
        int count,
        float radius,
        float minDistance)
    {
        var points = new List<Vector3>();
        int attempts = 0, maxAttempts = count * 100;

        while (points.Count < count && attempts < maxAttempts)
        {
            attempts++;
            Vector3 cand = center + (Random.insideUnitSphere.WithY(0f) * radius);
            if (NavMesh.SamplePosition(cand, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                bool tooClose = false;
                foreach (var p in points)
                    if (Vector3.Distance(p, hit.position) < minDistance) { tooClose = true; break; }
                if (!tooClose) points.Add(hit.position);
            }
        }

        if (points.Count < count)
            Debug.LogWarning($"[HordeSpawner] 원하는 {count}개 중 {points.Count}개만 찾았습니다.");

        return points;
    }

    public void DeSpawn()
    {
        foreach (var enemy in preSpawnedEnemies)
        {
            if (enemy == null) continue;
            var id = enemy.GetComponent<EnemyIdentifier>();
            if (id != null && !id.wasTrackingPlayer)
                EnemyPoolManager.Instance.ReturnToPool(id.Type, enemy, 0f);
        }
        preSpawnedEnemies.Clear();
    }
}

// Vector3 확장 메서드 (WithY) 예시
public static class Vector3Extensions
{
    public static Vector3 WithY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }
}
