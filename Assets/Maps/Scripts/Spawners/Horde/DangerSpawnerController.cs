using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using UnityEngine;

public class DangerSpawnerController : MonoBehaviour
{
    [SerializeField] private float spawnCooldown = 3f;
    private bool isActivated = false;
    private Coroutine spawnRoutine;
    private List<DangerSpawner> spawners;

    void Start()
    {
        GamePlayManager.instance.OnDangerAction += ActivateDangerSpawner;
        GamePlayManager.instance.OnPreDepartAction += DeactivateDangerSpawner; 
        spawners = GetComponentsInChildren<DangerSpawner>().ToList();
    }

    private void ActivateDangerSpawner()
    {
        if (isActivated) return;
        isActivated = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void DeactivateDangerSpawner()
    {
        if (!isActivated) return;
        isActivated = false;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnLoop()
    {
        while (isActivated)
        {
            TriggerSpawners();
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    private void TriggerSpawners()
    {
        foreach (var sp in spawners)
            sp.TrySpawn(GamePlayManager.instance.currentMapIndex);
    }
}
