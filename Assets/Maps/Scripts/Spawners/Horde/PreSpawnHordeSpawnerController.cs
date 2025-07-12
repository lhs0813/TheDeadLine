using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using Steamworks;
using UnityEngine;


public class PreSpawnHordeSpawnerController : MonoBehaviour
{
    public static bool isAnyRoomTriggered;

    private bool isSpawned;
    private bool isDespawned;
    private bool danger;

    [SerializeField] private Tile tileSpawning;
    [SerializeField] private List<PrespawnedHordeSpawner> spawners;

    private DungenCharacter character;

    void Start()
    {
        // 초기화
        character = FindAnyObjectByType<DungenCharacter>();
        tileSpawning = GetComponent<Tile>();
        spawners = GetComponentsInChildren<PrespawnedHordeSpawner>().ToList();
        danger = false;
        isDespawned = false;
        isSpawned = false;

        // 이벤트 구독
        MapGenerationManager.Instance.OnNavMeshBakeAction += InitializeSpawners;
        GamePlayManager.instance.OnDangerAction += () => danger = true;
    }

    void OnDestroy()
    {
        MapGenerationManager.Instance.OnNavMeshBakeAction -= InitializeSpawners;
        GamePlayManager.instance.OnDangerAction -= () => danger = true;
    }

    // 스포너 포인트 초기화
    public void InitializeSpawners()
    {
        spawners.RemoveAll(s => s == null);
        spawners.ForEach(s => s.InitializeSpawnPoints());
    }

    // 1) 플레이어가 문 앞 트리거에 진입했을 때 (Pre-spawn)
    public void OnPlayerDoorApproach(DoorController doorController)
    {
        if (isSpawned) return;

        isSpawned = true;

        // 다른 방의 미리 스폰된 적 제거
        foreach (var ctrl in FindObjectsByType<PreSpawnHordeSpawnerController>(FindObjectsSortMode.None))
            if (ctrl != this)
                ctrl.DespawnPreSpawnHorde();

        // None 상태에서만 내부 선제 스폰
        SpawnPreSpawnHorde();
        doorController.OpenDoor();
    }

    // 내부 선제 스폰 헬퍼
    private void SpawnPreSpawnHorde()
    {
        spawners.ForEach(s =>
        {
            if (s.gameObject.activeInHierarchy)
                s.TrySpawn(GamePlayManager.instance.currentMapIndex, false, danger);
        });
    }

    // 내부 스폰된 좀비 삭제
    public void DespawnPreSpawnHorde()
    {
        if (isDespawned) return;
        isDespawned = true;
        spawners.ForEach(s => s.DeSpawn());
    }
}
