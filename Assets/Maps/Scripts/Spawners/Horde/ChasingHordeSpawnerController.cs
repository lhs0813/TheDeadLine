using System;
using System.Collections.Generic;
using System.Linq;
using DunGen;
using UnityEngine;

/// <summary>
/// 스폰 즉시 플레이어를 추적하는 적들을 스폰하는 컨트롤러. Tile 프리팹 최상위에 붙여서 사용.
/// </summary>
public class ChasingHordeSpawnerController : MonoBehaviour
{
    ///<condition>
    ///타일 변경 발생시, 자신과 가까워진 경우. 
    ///타일 변경 발생시, 인접타일에 들어온 경우.

    DungenCharacter character;
    Tile spawningTile; //적이 스폰되는, 이 타일.
    List<ChasingHordeSpawner> spawners = new List<ChasingHordeSpawner>(); //하위에 존재하는 스포너 리스트.

    [Header("Cooldown Settings")]
    [Tooltip("한 번 스폰 후 다음 스폰까지 걸리는 시간(초)")]
    [SerializeField] private float spawnerCooldown = 30f;
    private float nextAllowedSpawnTime = 0f;
    
    void OnEnable()
    {
        character = FindAnyObjectByType<DungenCharacter>();
        spawningTile = GetComponent<Tile>();
        spawners = GetComponentsInChildren<ChasingHordeSpawner>().ToList();
        spawnerCooldown = 20f;

        character.OnTileChanged += ManagePlayerLocation; //플레이어 타일 변경 Event 구독.
    }

    void OnDisable()
    {
        character.OnTileChanged -= ManagePlayerLocation; //플레이어 타일 변경 Event 구독 해제.
    }

    /// <summary>
    /// 플레이어 위치 변경에 따른 Flag 변경 및 스폰 트리거.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="previousTile"></param>
    /// <param name="newTile"></param>
    private void ManagePlayerLocation(DungenCharacter character, Tile previousTile, Tile newTile)
    {
        //Null 체크: 이전 타일이 없으면, wasAdjacent = false 로 처리
        bool wasAdjacent = previousTile != null && previousTile.IsAdjacentTo(spawningTile);

        //지금 인접해 있는지
        bool nowAdjacent = newTile != null && newTile.IsAdjacentTo(spawningTile);

        //이전에 이미 인접해 있었으면, 플레이어는 해당 타일 기준으로 회전중
        if (wasAdjacent)
            return;

        //새로 인접해졌다면 스폰 시도
        if (nowAdjacent)
            TryActivateSpawner();

    }

    /// <summary>
    /// Cooldown 체크 후 실제 스폰 실행
    /// </summary>
    private void TryActivateSpawner()
    {
        if (Time.time < nextAllowedSpawnTime)
            return;

        ActivateHordeSpawner();
        nextAllowedSpawnTime = Time.time + spawnerCooldown;
    }

    /// <summary>
    /// 하위 모든 HordeSpawner 에게 TrySpawn 호출
    /// </summary>
    private void ActivateHordeSpawner()
    {
        foreach (var sp in spawners)
            sp.TrySpawn(GamePlayManager.instance.currentMapIndex);
    }
}
