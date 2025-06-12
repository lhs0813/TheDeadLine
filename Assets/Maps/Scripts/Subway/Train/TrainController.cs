using System.Collections;
using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TrainController : MonoBehaviour
{
    [SerializeField] private float trainDepartDelay = 3f; //문닫고, 실제 출발하기까지 걸리는 시간.
    [SerializeField] private float trainArriveDelay = 1f; //열차가 플랫폼에 도착하고, 문이 열리기 시작하는 시간.
    [SerializeField] private float currentSpeed; //현재속도
    [SerializeField] private float trainMaxSpeed; //최고속도
    [SerializeField] private float trainMinSpeed; //최저속도
    [SerializeField] private float trainAccelerateRate; //가속도
    [SerializeField] private bool isMoving; //운행중.
    [SerializeField] private bool isStopping; //멈추는중.

    [SerializeField] private Collider trainInteriorZone; // 플레이어가 안에 있는지 확인할 트리거 콜라이더
    [SerializeField] private Transform trainTransform;
    TrainDoorController trainDoorController;
    PlayerHordeTrigger playerHordeTrigger;

    TrainSoundController trainSoundController;

    void Start()
    {
        trainDoorController = GetComponentInChildren<TrainDoorController>();
        playerHordeTrigger = FindAnyObjectByType<PlayerHordeTrigger>();
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        trainSoundController = GetComponentInChildren<TrainSoundController>();
    }

    /// <summary>
    /// GameManager -> 스테이지 시간 종료. 
    /// </summary>
    public void TrainDepart()
    {
        StartCoroutine(TrainDepartCoroutine());

    }

    public void TrainArrive()
    {
        StartCoroutine(TrainArriveCoroutine());
    }

    IEnumerator TrainDepartCoroutine()
    {
        trainDoorController.CloseDoor();
        trainSoundController.PlayDoorClose();

        yield return new WaitForSeconds(trainDepartDelay); // 문 닫히는 시간보다 길게

        trainSoundController.PlayTrainRunning();
        CheckAndAttachPlayer();
        isMoving = true;
    }

    IEnumerator TrainArriveCoroutine()
    {
        isMoving = false;
        isStopping = false;


        yield return new WaitForSeconds(trainArriveDelay);
        DetachPlayer();
        trainDoorController.OpenDoor();
        trainSoundController.PlayDoorOpen();

    }

    void Update()
    {
        ManageSpeed();
        MoveTrain();
    }

    void ManageSpeed()
    {
        if (!isMoving)
        {
            currentSpeed = 0;
            return;
        }

        if (!isStopping && currentSpeed < trainMaxSpeed) //가속중
        {
            currentSpeed += trainAccelerateRate * Time.deltaTime;
        }
        else if (isStopping && currentSpeed > trainMinSpeed)
        {
            currentSpeed -= trainAccelerateRate * Time.deltaTime;
        }
        else
        {
            //변화 없음.
        }
    }


    void MoveTrain()
    {
        if (!isMoving) return; //역 정차시 움직이지 않음.

        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime); // 또는 원하는 방향
    }

    /// <summary>
    /// 출발후 일정 시간 이후 or 특정 지점에서 이동.
    /// </summary>
    public void MoveToWaitingRail()
    {
        Transform waitingRailStartTransform = FindAnyObjectByType<WaitingRailStartPoint>().transform;

        transform.position = waitingRailStartTransform.position;
    }

    /// <summary>
    /// 맵 로딩 완료 And 운행 시간 종료.
    /// </summary>
    public void MoveToStageRail()
    {
        Transform stageRailStartTransform = FindAnyObjectByType<StageRailStartPoint>().transform;

        transform.position = stageRailStartTransform.position;

        isStopping = true;

        trainSoundController.PlayTrainArriving();
    }

    public bool CheckAndAttachPlayer()
    {
        // Debug.Log("Check and Attack Player");
        // Collider[] hits = Physics.OverlapBox(
        //     trainInteriorZone.bounds.center,
        //     trainInteriorZone.bounds.extents,
        //     trainInteriorZone.transform.rotation
        //     //LayerMask.GetMask("Player") // 플레이어 전용 레이어 사용 권장
        // );

        // foreach (var hit in hits)
        // {
        //     Debug.Log(hit.name);

        //     if (hit.CompareTag("Player"))
        //     {
        //         hit.transform.SetParent(trainTransform);
        //         Debug.Log("✅ Player attached to train.");
        //     }
        // }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (trainInteriorZone.bounds.Contains(player.transform.position))
        {
            player.transform.SetParent(trainTransform);
            return true;
        }

        return false;
    }

    void DetachPlayer()
    {
        FindAnyObjectByType<CharacterManager>().transform.SetParent(null);
    }


}
