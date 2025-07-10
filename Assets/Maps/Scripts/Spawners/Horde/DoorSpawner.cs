using System.Collections;
using UnityEngine;

public class DoorSpawner : MonoBehaviour
{
    public DoorController doorController;
    void OnTriggerEnter(Collider other)
    {
        // GetComponentInParent<T>()가 null이 아니면 id에 할당
        if (other.GetComponentInParent<EnemyIdentifier>() is EnemyIdentifier id)
        {
            id.isTileChanged = true;
            return;
        }

        if (other.CompareTag("Player"))
        {
            GetComponentInParent<PreSpawnHordeSpawnerController>().OnDoorSpawnTriggered(this, doorController);
        }
    }

    public void TrySpawn(int mapIndex, bool danger)
    {
        StartCoroutine(SpawnRoutine(mapIndex, danger));
    }

    private IEnumerator SpawnRoutine(int mapIndex, bool danger)
    {
        int spawnCount = MapGenCalculator
            .GetCreatureSpawnCountRangePerSpawner(mapIndex)
            .GetRandom(new DunGen.RandomStream());
        if (danger)
            spawnCount *= 2;

        for (int i = 0; i < spawnCount / 3; i++)
        {
            EnemyType type = HordeSpawnBuilder.RollEnemyType(mapIndex);
            EnemyPoolManager.Instance.Spawn(type, transform.position, Quaternion.identity, false);

            // 한 프레임만 기다렸다가 다음 루프로 넘어감
            yield return null;
        }
    }
}
