using System;
using UnityEngine;

public class DangerTImer : MonoBehaviour
{
    // 빌드된 컨트롤러들이 읽어갈 static 플래그
    public static bool isDangerSpawnable;

    [SerializeField] float dangerSpawnCooldown = 30f;
    float currentDangerSpawnTimer;
    bool isDanger;

    void Start()
    {
        isDangerSpawnable = false;
        currentDangerSpawnTimer = 0f;
        GamePlayManager.instance.OnDangerAction    += EnableDangerState;
        GamePlayManager.instance.OnPreDepartAction += DisableDangerState;
    }

    void Update()
    {
        if (!isDanger) return;

        // 아직 spawn 불가능 상태라면
        if (!isDangerSpawnable)
        {
            currentDangerSpawnTimer += Time.deltaTime;
            if (currentDangerSpawnTimer >= dangerSpawnCooldown)
            {
                // 타이머 리셋과 동시에 한번만 true
                isDangerSpawnable      = true;
                currentDangerSpawnTimer = 0f;
            }
        }
    }

    private void EnableDangerState()
    {
        isDanger = true;
        isDangerSpawnable = false;
        currentDangerSpawnTimer = 0f;
    }

    private void DisableDangerState()
    {
        isDanger = false;
        isDangerSpawnable = false;
        currentDangerSpawnTimer = 0f;
    }

    void OnDestroy()
    {
        GamePlayManager.instance.OnDangerAction    -= EnableDangerState;
        GamePlayManager.instance.OnPreDepartAction -= DisableDangerState;
    }
}
