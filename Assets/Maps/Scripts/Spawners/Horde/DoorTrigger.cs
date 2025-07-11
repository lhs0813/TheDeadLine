using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Tooltip("문 앞 트리거에 반응할 스포너 컨트롤러")]
    public PreSpawnHordeSpawnerController spawnerController;
    public DoorController doorController;
    private void Start()
    {
        spawnerController = GetComponentInParent<PreSpawnHordeSpawnerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // 플레이어가 문 앞 트리거에 들어왔을 때 스포너에 알림
        spawnerController.OnPlayerDoorApproach(doorController);
        
        // 한 번만 동작하도록 자신(트리거)을 제거
        Destroy(gameObject);
    }
}