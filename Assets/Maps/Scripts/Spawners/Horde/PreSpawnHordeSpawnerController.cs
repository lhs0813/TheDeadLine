using System;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using UnityEngine;

/// <summary>
/// 플레이어가 근처 타일에 진입했을 때, 적을 스폰.
/// </summary>
public class PreSpawnHordeSpawnerController : MonoBehaviour
{
    //선제 스폰된 상태. 다음 Tile이 자기 자신인지 확인하기 위함.
    [SerializeField] private bool isBeingPreSpawned;
    //한번 트리거된 선제 스폰 방은 다시 트리거 되지 않음.
    [SerializeField] private bool isAlreadyTriggered;
    //해당 스크립트가 같이 붙어있는 Tile Component.
    [SerializeField] private Tile tileSpawning;
    [SerializeField] private List<PrespawnedHordeSpawner> spawners;
    [SerializeField] private int mapIndex;
    [SerializeField] private bool danger;
    DungenCharacter character;

    void Start()
    {
        //초기화 (DungenCharacter, Tile.)
        character = FindAnyObjectByType<DungenCharacter>();
        tileSpawning = GetComponent<Tile>();
        spawners = GetComponentsInChildren<PrespawnedHordeSpawner>().ToList();

        //OnTileChanged Event는 DungenCharacter 자기자신, 이전 타일, 현재 타일을 줌.
        character.OnTileChanged += ManagePlayerLocation;
        MapGenerationManager.Instance.OnNavMeshBakeAction += InitializeSpawners;
        GamePlayManager.instance.OnDangerAction += ChangeToDangerState;

        mapIndex = GamePlayManager.instance.currentMapIndex;
        danger = false;
    }

    private void ChangeToDangerState()
    {
        isAlreadyTriggered = false;
        danger = true;
    }

    public void InitializeSpawners()
    {
        // 1) 파괴된(=null) 스포너만 걸러내기
        spawners.RemoveAll(s => s == null);

        // 2) 남은 스포너만 초기화
        foreach (var sp in spawners)
            sp.InitializeSpawnPoints();
    }

    private void ManagePlayerLocation(DungenCharacter character, Tile previousTile, Tile newTile)
    {
        if (newTile == null) return;

        if (isAlreadyTriggered) //생성되고, 플레이어가 같은 방까지 들어온 적이 있음.
        {
            if (!newTile.IsAdjacentTo(tileSpawning) && newTile != tileSpawning)
            {
                //플레이어가 들어온적이 있는데, 플레이어가 두 칸 떨어진 공간까지 이동.
                DespawnPreSpawnHorde(); //생성된 적들을 제거.
                isBeingPreSpawned = false; //다시 생성될 수 있음.
            }
            else
            {
                return; //적들이 사라지지 않아도 됨. 반환           
            }

        }
        else
        {
            if (!isBeingPreSpawned) //스폰되지는 않은 상태.
            {
                if (newTile.IsAdjacentTo(tileSpawning)) //인접타일에 플레이어가 있음.
                {
                    isBeingPreSpawned = true; //플래그 처리.
                    SpawnPreSpawnHorde(); //스폰.
                }
                else //스폰도 안되었고, 인접타일에 플레이어도 없음.
                {
                    return; //반환.
                }
            }
            else //스폰됨. 플레이어가 이 타일에 들어올 때까지 기다려야 함.
            {
                if (newTile == tileSpawning) //적이 스폰된 상태에서, 해당 타일에 플레이어가 들어옴.
                {
                    isAlreadyTriggered = true; //더 이상 이 방에서 선제 스폰이 발생하지 않음. flag 설정.
                }
                else if (!newTile.IsAdjacentTo(tileSpawning)) //스폰이 되었는데, 플레이어가 인접하지 않은 타일로 감.
                {
                    isBeingPreSpawned = false; //다시 플레이어가 접근할 때 까지 대기. flag 설정.
                    DespawnPreSpawnHorde(); //생성된 적들을 제거.
                }
                else //newTile이 인접타일인 경우, 주위를 돈 것이라고 해석. 스폰된 상태를 유지.
                {
                    if (!isBeingPreSpawned)
                    {
                        isBeingPreSpawned = true;
                    }
                }
            }

        }

    }

    /// <summary>
    /// 선제적 적 스폰 처리.
    /// </summary>
    private void SpawnPreSpawnHorde()
    {
        foreach (var sp in spawners)
        {
            if(sp.gameObject.activeInHierarchy)
            sp.TrySpawn(GamePlayManager.instance.currentMapIndex, danger);
        }

    }

    /// <summary>
    /// 플레이어가 멀어진 경우, 스폰된 적들을 반환.
    /// </summary>
    private void DespawnPreSpawnHorde()
    {
        foreach (var sp in spawners)
            sp.DeSpawn();
    }

    void OnDestroy()
    {
        character.OnTileChanged -= ManagePlayerLocation;
        MapGenerationManager.Instance.OnNavMeshBakeAction -= InitializeSpawners;
        GamePlayManager.instance.OnDangerAction -= ChangeToDangerState;        
    }
}
