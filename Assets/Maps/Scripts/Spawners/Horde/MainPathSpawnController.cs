using System;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class MainPathSpawnController : MonoBehaviour
{
    DungenCharacter character;
    Tile tileSpawning;
    List<MainPathSpawner> spawners;


    bool upperSpawned;
    bool underSpawned;
    bool danger;

    void Start()
    {
        //초기화 (DungenCharacter, Tile.)
        character = FindAnyObjectByType<DungenCharacter>();
        tileSpawning = GetComponent<Tile>();
        // false를 넘기면, 오직 활성화된(ActiveSelf) 자식만 가져옵니다.
        spawners = GetComponentsInChildren<MainPathSpawner>(includeInactive: false).ToList();


        character.OnTileChanged += ManagePlayerLocation;
        MapGenerationManager.Instance.OnNavMeshBakeAction += InitializeSpawners;
        GamePlayManager.instance.OnDangerAction += ChangeToDangerState;

        //flag Initialize.
        upperSpawned = false;
        underSpawned = false;
        danger = false;
    }

    private void ManagePlayerLocation(DungenCharacter character, Tile previousTile, Tile newTile)
    {
        // 안전성 체크
        if (newTile == null || previousTile == null)
            return;

        // 메인 경로가 아니면 처리하지 않음
        if (!newTile.IsMainPath())
            return;

        // 깊이 계산
        int prevDepth  = previousTile.GetDeepness();
        int newDepth   = newTile.GetDeepness();
        int spawnDepth = tileSpawning.GetDeepness();
        int delta      = newDepth - prevDepth;

        // 상승/하강 분기 (플래그 검사 포함)
        if (delta > 0 && newDepth == spawnDepth - 1 && !upperSpawned)
        {
            UpperSpawn();
            upperSpawned = true;
        }
        else if (delta < 0 && newDepth == spawnDepth + 2 && !underSpawned)
        {
            UnderSpawn();
            underSpawned = true;
        }
        else if (delta == 0)
        {
            // 같은 깊이(수평 이동) 시 필요하다면 처리
        }
        else
        {
            Debug.LogWarning($"[{nameof(MainPathSpawnController)}] Unexpected depth jump: {delta}");
        }
    }


    private void UpperSpawn()
    {
        foreach (var s in spawners)
        {
            s.MainSpawn(GamePlayManager.instance.currentMapIndex, false);
        }
    }

    
    private void UnderSpawn()
    {
        foreach (var s in spawners)
        {
            s.MainSpawn(GamePlayManager.instance.currentMapIndex, true);
        }      
    }

    public void InitializeSpawners()
    {
        // 1) 파괴된(=null) 스포너만 걸러내기
        spawners.RemoveAll(s => s == null);

        // 2) 남은 스포너만 초기화
        foreach (var sp in spawners)
            sp.InitializeSpawnPoints(GamePlayManager.instance.currentMapIndex);
    }

    private void ChangeToDangerState()
    {
        danger = true;
        upperSpawned = false;
        underSpawned = false;
    }
}
