using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using Steamworks;
using UnityEngine;

public enum PreSpawnRoomState
{
    None,               // 아무 일 없음
    Releasing,          // Release 콜라이더로 열려서 스폰 중
    PlayerApproached    // 플레이어가 문 앞에 접근해서 내부 선제 스폰 & 문이 열림
}

public class PreSpawnHordeSpawnerController : MonoBehaviour
{
    public static bool isAnyRoomTriggered;

    [HideInInspector] public PreSpawnRoomState state;
    private bool isDespawned;
    private bool danger;

    [SerializeField] private Tile tileSpawning;
    [SerializeField] private List<PrespawnedHordeSpawner> spawners;

    private DungenCharacter character;

    void Start()
    {
        // 초기화
        character    = FindAnyObjectByType<DungenCharacter>();
        tileSpawning = GetComponent<Tile>();
        spawners     = GetComponentsInChildren<PrespawnedHordeSpawner>().ToList();
        danger       = false;
        state        = PreSpawnRoomState.None;
        isDespawned  = false;

        // 이벤트 구독
        character.OnTileChanged                      += ManagePlayerLocation;
        MapGenerationManager.Instance.OnNavMeshBakeAction += InitializeSpawners;
        GamePlayManager.instance.OnDangerAction          += () => danger = true;
    }

    void OnDestroy()
    {
        character.OnTileChanged                      -= ManagePlayerLocation;
        MapGenerationManager.Instance.OnNavMeshBakeAction -= InitializeSpawners;
        GamePlayManager.instance.OnDangerAction          -= () => danger = true;
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
        // 이미 문 앞에서 열림 상태라면 아무것도 하지 않음
        if (state == PreSpawnRoomState.PlayerApproached)
        {
            doorController.OpenDoor();
            return;
        }


        // 다른 방의 미리 스폰된 적 제거
        foreach (var ctrl in FindObjectsByType<PreSpawnHordeSpawnerController>(FindObjectsSortMode.None))
            if (ctrl != this)
                ctrl.DespawnPreSpawnHorde();

        // Release 흐름 도중 접근이라면, 단순히 문만 연 채로 전환
        if (state == PreSpawnRoomState.Releasing)
        {
            state = PreSpawnRoomState.PlayerApproached;
            doorController.OpenDoor();
            return;
        }

        // None 상태에서만 내부 선제 스폰
        SpawnPreSpawnHorde();
        state = PreSpawnRoomState.PlayerApproached;
        doorController.OpenDoor();


    }

    // 2) Release 콜라이더 진입 시 (Release-spawn)
    public void OnDoorSpawnTriggered(DoorSpawner spawner, DoorController doorController)
    {
        // 이미 Release 또는 Approach 상태라면 무시
        if (state != PreSpawnRoomState.None || isAnyRoomTriggered)
            return;

        isAnyRoomTriggered = true;
        state              = PreSpawnRoomState.Releasing;
        isDespawned        = false;

        // 내부 + 외부 스폰
        SpawnPreSpawnHorde();
        spawner.TrySpawn(GamePlayManager.instance.currentMapIndex, danger);

        // 문 열고 닫기 예약
        doorController.OpenDoor();
        StartCoroutine(DoorSpawnCoroutine(doorController));

        // 한 번만 트리거되도록 비활성화
        spawner.GetComponent<Collider>().enabled = false;
    }

    private IEnumerator DoorSpawnCoroutine(DoorController doorController)
    {
        yield return new WaitForSeconds(3f);

        // 아직 PlayerApproached 전환이 안 되었으면 닫고 해제
        if (state == PreSpawnRoomState.Releasing)
        {
            doorController.CloseDoor();
            yield return new WaitForSeconds(1f);

            if (state == PreSpawnRoomState.Releasing)
            {
                DespawnDoorSpawnHorde();
                state = PreSpawnRoomState.None;                
            }

        }

        yield return new WaitForSeconds(5f);

        isAnyRoomTriggered = false;
    }

    // 3) 플레이어가 방을 떠났을 때(Approach 흐름 해제)
    private void ManagePlayerLocation(DungenCharacter _, Tile __, Tile newTile)
    {
        if (newTile == null) return;

        // PlayerApproached 상태에서 방을 완전히 벗어나면 내부 좀비만 제거
        if (state == PreSpawnRoomState.PlayerApproached
            && !newTile.IsAdjacentTo(tileSpawning)
            && newTile != tileSpawning)
        {
            DespawnPreSpawnHorde();
            state = PreSpawnRoomState.None;
        }
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

    // Release 스폰된 방 밖 좀비 삭제
    private void DespawnDoorSpawnHorde()
    {
        spawners.ForEach(s => s.DeSpawnUnExitedHorde());
    }
}
