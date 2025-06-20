using UnityEngine;
using UnityEngine.AI;

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

            // 1) 원래 스포너 위치를 기준으로 가장 가까운 NavMesh 점을 찾습니다.
            Vector3 basePos = transform.position;
            NavMeshHit hit;
            float maxDistance = 5f; // 검색 반경
            Vector3 spawnPos = basePos;

            if (NavMesh.SamplePosition(basePos, out hit, maxDistance, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }
            // (실패 시엔 원래 위치(transform.position)를 사용)

            //스폰
            GameObject enemy = EnemyPoolManager
                .Instance
                .Spawn(type, spawnPos, Quaternion.identity, false);
        }
    }
}
