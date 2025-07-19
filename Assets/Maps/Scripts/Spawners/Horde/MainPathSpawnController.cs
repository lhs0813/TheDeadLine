using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using UnityEngine;

public class MainPathSpawnController : MonoBehaviour
{
    DungenCharacter character;
    Tile tileSpawning;
    public List<MainPathSpawner> spawners;

    int spawnDeepness;
    int mapIndex;
    bool upperSpawned;
    bool underSpawned;
    bool danger;

    bool canSpawn;
    public float spawnCooldown = 5f;

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


        mapIndex = GamePlayManager.instance.currentMapIndex;
        //flag Initialize.
        upperSpawned = false;
        underSpawned = false;
        danger = false;

        canSpawn = true;

        spawnDeepness = tileSpawning.GetDeepness();
        Debug.Log(spawnDeepness);
    }

    private void ManagePlayerLocation(DungenCharacter character, Tile previousTile, Tile newTile)
    {
        if (!canSpawn) return;
        // 안전성 체크
        if (newTile == null)
            return;

        // 메인 경로가 아니면 처리하지 않음
        if (!newTile.IsMainPath())
            return;

        // 깊이 계산
        int prevDepth = previousTile.GetDeepness();
        int newDepth = newTile.GetDeepness();
        int delta = newDepth - prevDepth;

        if (!previousTile.IsMainPath()) //방 밖으로 나온 경우.//
        {
            if (GamePlayManager.instance.goingUp) //상승기조. 윗쪽 계단에서 스폰되도록.
            {
                if (newDepth == spawnDeepness - 1)
                {
                    UnderSpawn();
                    StartCoroutine(SpawnCooldown());
                    return;
                }
            }
            else //하강기조
            {
                if (newDepth == spawnDeepness + 1)
                {
                    UnderSpawn();
                    StartCoroutine(SpawnCooldown());
                    return;
                }
            }
        }

        // 상승/하강 분기 (플래그 검사 포함)
        if (delta > 0 && newDepth == spawnDeepness - 1 && !upperSpawned)
        {
            UpperSpawn();
            StartCoroutine(SpawnCooldown());
            upperSpawned = true;
        }
        else if (delta < 0 && newDepth == spawnDeepness + 1 && !underSpawned)
        {
            UnderSpawn();
            StartCoroutine(SpawnCooldown());
            underSpawned = true;
        }
        else if (newTile != tileSpawning && Mathf.Abs(newDepth - spawnDeepness) >= 2)
        {
            DeSpawn();
        }
        else if (delta == 0)
        {
            // 같은 깊이(수평 이동) 시 필요하다면 처리
        }
        else
        {
            //Debug.LogWarning($"[{nameof(MainPathSpawnController)}] Unexpected depth jump: {delta}");
        }
    }

    private IEnumerator SpawnCooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(spawnCooldown);
        canSpawn = true;
    }


    private void UpperSpawn()
    {
        foreach (var s in spawners)
        {
            s.MainSpawn(mapIndex, false, danger, spawnDeepness);
        }
    }


    private void UnderSpawn()
    {
        foreach (var s in spawners)
        {
            s.MainSpawn(mapIndex, true, danger, spawnDeepness);
        }
    }

    private void DeSpawn()
    {


        foreach (var s in spawners)
        {
            s.DeSpawn(spawnDeepness) ;
        }        
    }

    public void InitializeSpawners()
    {
        // 1) 파괴된(=null) 스포너만 걸러내기
        spawners.RemoveAll(s => s == null);

        // 2) 남은 스포너만 초기화
        foreach (var sp in spawners)
            sp.InitializeSpawnPoints(mapIndex);
    }

    private void ChangeToDangerState()
    {
        danger = true;
        upperSpawned = false;
        underSpawned = false;
    }

    void OnDestroy()
    {
        character.OnTileChanged -= ManagePlayerLocation;
        MapGenerationManager.Instance.OnNavMeshBakeAction -= InitializeSpawners;
        GamePlayManager.instance.OnDangerAction -= ChangeToDangerState;        
    }
}
