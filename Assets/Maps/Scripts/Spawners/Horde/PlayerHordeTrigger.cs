using System.Collections.Generic;
using DunGen;
using UnityEngine;

public class PlayerHordeTrigger : MonoBehaviour
{
    private bool activated = false;
    private int currentMapIndex;

    [Header("Spawn Cooldown & Range")]
    [SerializeField] private FloatRange currentCreatureSpawnCooldownRange;
    [SerializeField] private FloatRange currentCreatureSpawnRadiusRange;

    private float nextTriggerTime;
    private Transform playerTransform;

    private List<Transform> currentValidTargets = new();

    void Start()
    {
        playerTransform = this.transform;

        ActivatePlayerHordeTrigger(1);
    }

    //TODO : 기차역 도착시 호출해주어야함.
    public void ActivatePlayerHordeTrigger(int mapIndex)
    {
        currentMapIndex = mapIndex;
        activated = true;
        nextTriggerTime = Time.time;

        currentCreatureSpawnCooldownRange = MapGenCalculator.GetHordeTriggerCooldownRange(mapIndex);
        currentCreatureSpawnRadiusRange = MapGenCalculator.GetHordeTriggerRadiusFromPlayerRange();
    }

    //TODO : 기차역 출발시 호출해주어야함.
    public void DeactivatePlayerHordeTrigger()
    {
        activated = false;
    }

    void Update()
    {
        if (!activated)
            return;

        if (Time.time >= nextTriggerTime)
        {
            TriggerNearbyHordeRooms();
            nextTriggerTime = Time.time + Random.Range(currentCreatureSpawnCooldownRange.Min, currentCreatureSpawnCooldownRange.Max);
        }
    }

    private float currentSectorCenterAngle = 0f;       // 섹터 중앙(월드 Y축 기준)
    [SerializeField] private float sectorHalfAngle = 70f;  // 절반 시야각

    private void TriggerNearbyHordeRooms()
    {
        currentValidTargets.Clear();
        Collider[] colliders = Physics.OverlapSphere(
            playerTransform.position,
            MapGenConstants.MaxCreatureSpawnRadius,
            LayerMask.GetMask("HordeSpawner")
        );

        // 1) 원하는 섹터 각도 기준으로 필터
        foreach (var col in colliders)
        {
            Vector3 dir = (col.transform.position - playerTransform.position).normalized;
            float angle = Vector3.SignedAngle(playerTransform.forward, dir, Vector3.up);
            if (Mathf.Abs(Mathf.DeltaAngle(angle, currentSectorCenterAngle)) > sectorHalfAngle)
                continue;

            if (col.TryGetComponent(out HordeSpawner sp))
                currentValidTargets.Add(col.transform);
        }

        // 2) 한 웨이브당 최대 N개만 랜덤/순차 선택
        int waveCount = Mathf.Min(5, currentValidTargets.Count);
        for (int i = 0; i < waveCount; i++)
        {
            int idx = Random.Range(0, currentValidTargets.Count);
            var trg = currentValidTargets[idx];
            trg.GetComponent<HordeSpawner>().TrySpawn(currentMapIndex);
            currentValidTargets.RemoveAt(idx);
        }

        // 3) 다음 웨이브 섹터 회전 (예: 90°씩 시계방향)
        currentSectorCenterAngle = (currentSectorCenterAngle + 90f) % 360f;
    }


    // 👇 Gizmo 시각화
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
            playerTransform = transform;

        Vector3 center = playerTransform.position;
        Vector3 forward = playerTransform.forward;

        // 🔵 원형 반경 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, MapGenConstants.MinCreatureSpawnRadius);
        Gizmos.DrawWireSphere(center, MapGenConstants.MaxCreatureSpawnRadius);

        // 🔺 시야각 부채꼴 표시
        float angle = MapGenConstants.EnemyTriggerAngleFromPlayer * 0.5f;
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(center, leftDir * MapGenConstants.MaxCreatureSpawnRadius);
        Gizmos.DrawRay(center, rightDir * MapGenConstants.MaxCreatureSpawnRadius);

        // 🔴 조건에 맞는 타겟까지 선
        Gizmos.color = Color.red;
        foreach (var target in currentValidTargets)
        {
            Gizmos.DrawLine(center, target.position);
        }
    }

    
    // private void TriggerNearbyHordeRooms()
    // {
    //     currentValidTargets.Clear();

    //     Collider[] colliders = Physics.OverlapSphere(playerTransform.position, MapGenConstants.MaxCreatureSpawnRadius, LayerMask.GetMask("HordeSpawner"));

    //     foreach (var collider in colliders)
    //     {
    //         Vector3 toTrigger = collider.transform.position - playerTransform.position;
    //         float distance = toTrigger.magnitude;

    //         if (distance < MapGenConstants.MinCreatureSpawnRadius || distance > MapGenConstants.MaxCreatureSpawnRadius)
    //             continue;

    //         float angle = Vector3.Angle(playerTransform.forward, toTrigger.normalized);
    //         if (angle > MapGenConstants.EnemyTriggerAngleFromPlayer * 0.5f)
    //             continue;

    //         if (collider.TryGetComponent(out HordeSpawner spawner))
    //         {
    //             spawner.TrySpawn(currentMapIndex);
    //             currentValidTargets.Add(collider.transform);
    //         }
    //     }
    // }
}
