using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Akila.FPSFramework;
using Unity.AI.Navigation;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    [SerializeField] private float trainDepartDelay = 3f; //문닫고, 실제 출발하기까지 걸리는 시간.
    [SerializeField] private float trainArriveDelay = 1f; //열차가 플랫폼에 도착하고, 문이 열리기 시작하는 시간.
    [SerializeField] private float doorConnectDelay = 1f;
    [SerializeField] private float currentSpeed; //현재속도
    [SerializeField] private float trainMaxSpeed; //최고속도
    [SerializeField] private float trainMinSpeed; //최저속도
    [SerializeField] private float trainAccelerateRate; //가속도
    [SerializeField] private bool isMoving; //운행중.
    [SerializeField] private bool isStopping; //멈추는중.

    [SerializeField] private Collider trainInteriorZone; // 플레이어가 안에 있는지 확인할 트리거 콜라이더
    [SerializeField] private Transform trainTransform;
    TrainDoorController trainDoorController;
    TrainSoundController trainSoundController;

    void Start()
    {
        trainDoorController = GetComponentInChildren<TrainDoorController>();
        trainSoundController = GetComponentInChildren<TrainSoundController>();

        GamePlayManager.instance.OnStationArriveAction += DisableAllLights;
        GamePlayManager.instance.OnTrainAccelerationAction += TrainAcceleration;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += EnableAllLights;

        trainDoorController.OpenDoor();
    }

    public void DoorClose()
    {
        List<NavMeshLink> links = GetComponentsInChildren<NavMeshLink>().ToList();
        foreach (var link in links)
        {
            link.enabled = false;
        }

        trainDoorController.CloseDoor();
        trainSoundController.PlayDoorClose();
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
        GetComponent<NavMeshSurface>().enabled = false;

        yield return new WaitForSeconds(trainDepartDelay); // 문 닫히는 시간보다 길게

        trainSoundController.PlayTrainRunning();
        isMoving = true;

        Debug.Log("3초 뒤 대기 레일로 이동");
        StartCoroutine(DelayedStageRail(3f));
    }

    IEnumerator DelayedStageRail(float delay)
    {
        yield return new WaitForSeconds(delay);
        GamePlayManager.instance.GoWaitingState();
    }

    IEnumerator TrainArriveCoroutine()
    {
        //DetachPlayer();
        isMoving = false;
        isStopping = false;


        //yield return null;
        yield return new WaitForSeconds(trainArriveDelay);


        trainDoorController.OpenDoor();
        trainSoundController.PlayDoorOpen();


        yield return new WaitForSeconds(doorConnectDelay);

        List<NavMeshLink> links = GetComponentsInChildren<NavMeshLink>().ToList();
        foreach (var link in links)
        {
            link.enabled = true;
        }
        GetComponent<NavMeshSurface>().enabled = true;
        GetComponent<NavMeshSurface>().BuildNavMesh();

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

    [SerializeField] private float arriveDuration = 3f;    // 감속 후 멈출까지 걸릴 시간
    private Vector3 arriveTarget;

    /// <summary>
    /// 맵 로딩 완료 And 운행 시간 종료.
    /// 즉시 이동 대신 부드러운 감속 이동 시작.
    /// </summary>
    public void MoveToStageRail()
    {
        //플레이어를 이동.
        Transform stageRailStartTransform = FindAnyObjectByType<StageRailStartPoint>().transform;

        transform.position = stageRailStartTransform.position;

        //목표 위치 저장
        arriveTarget = FindAnyObjectByType<TrainStopPoint>().transform.position;

        //감속 사운드 (도착 사운드)
        trainSoundController.PlayTrainArriving();

        //기존 Update 속도 제어 사용 중지
        isMoving = false;
        isStopping = false;

        //부드러운 감속+도착 코루틴
        StartCoroutine(ArriveAtTarget(arriveTarget, arriveDuration));


    }

    private IEnumerator ArriveAtTarget(Vector3 target, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        // 초기 속도를 현재 실제 속도로 가져오거나, 최고속도로 세팅
        float startSpeed = currentSpeed = trainMaxSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // ease-out 감속: 1 - (1-t)^2
            float ease = 1f - Mathf.Pow(1f - t, 2f);

            // 위치 보간
            transform.position = Vector3.Lerp(startPos, target, ease);

            // 속도 피드백 (원하면 사운드 볼륨/피치 등에 쓰세요)
            currentSpeed = Mathf.Lerp(startSpeed, 0f, ease);

            yield return null;
        }

        // 정확히 도착
        transform.position = target;
        currentSpeed = 0f;

        // 그 다음에 도착 처리 (문 열기, NavMesh 빌드 등)
        GamePlayManager.instance.GoCombatState();
    }

    public bool CheckPlayerInside()
    {
        // 1) 내부 존의 AABB와 회전을 이용해 겹치는 콜라이더 전부 수집
        var b = trainInteriorZone.bounds;
        Collider[] hits = Physics.OverlapBox(
            b.center,
            b.extents,
            trainInteriorZone.transform.rotation
        );

        // 3) 플레이어 체크
        var player = GameObject.FindGameObjectWithTag("Player");
        if (b.Contains(player.transform.position))
        {
            player.transform.SetParent(trainTransform);
            return true;
        }

        return false;
    }


    // public bool CheckEnemyInside()
    // {
    //     // 1) 내부 존의 AABB와 회전을 이용해 겹치는 콜라이더 전부 수집
    //     var b = trainInteriorZone.bounds;
    //     Collider[] hits = Physics.OverlapBox(
    //         b.center,
    //         b.extents,
    //         trainInteriorZone.transform.rotation,
    //         LayerMask.GetMask("Monster")
    //     );

    //     bool isEnemyInside = false;
    //     int enemyInsideCount = 0;

    //     foreach (var col in hits)
    //     {
    //         EnemyIdentifier id;

    //         if ((id = col.GetComponentInParent<EnemyIdentifier>()) != null)
    //         {
    //             id.transform.SetParent(trainTransform);
    //             isEnemyInside = true;
    //             enemyInsideCount++;
    //         }
    //     }

    //     Debug.Log($"내부의 적 수 : {enemyInsideCount}");
    //     return isEnemyInside;
    // }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GamePlayManager.instance.CheckDepart();
            //other.transform.SetParent(trainTransform);
        }
    }

    public void TrainAcceleration(float f)
    {
        trainSoundController.PlayTrainAccelerating();
    }
    
    #region Light Controller.

    [SerializeField] List<Light> trainLights;

    void DisableAllLights(float f)
    {
        foreach (var l in trainLights)
        {
            l.gameObject.SetActive(false);
        }
    }

    void EnableAllLights()
    {
        foreach (var l in trainLights)
        {
            l.gameObject.SetActive(true);
        }        
    }

    #endregion

    void OnDisable()
    {
        GamePlayManager.instance.OnStationArriveAction -= DisableAllLights;
        GamePlayManager.instance.OnTrainAccelerationAction -= TrainAcceleration;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= EnableAllLights;
    }
}
