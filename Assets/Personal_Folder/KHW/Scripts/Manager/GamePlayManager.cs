using System;
using System.Collections;
using System.Threading.Tasks;
using DunGen;
using UnityEngine;

public enum GameState
{
    Waiting,
    Entering,
    Combat,
    Danger,
    PreDeparting,
    Departing
}

public class GamePlayManager : MonoBehaviour
{
    public int currentMapIndex = 0;

    public GameState currentGameState;

    public static GamePlayManager instance;
    public RuntimeDungeon runtimeDungeon;
    public TrainController trainController;
    public static float Timer;
    private float nextWaitingEndtime;
    private float nextNormalCombatEndTime;
    private bool newMapReady; //맵 생성 완료 알림
    [SerializeField] private float waitingDuration = 20f; // 열차 대기 시간
    [SerializeField] private float normalCombatDuration = 180f; //전투시간. 
    [SerializeField] GamePlayManagementUI gamePlayManagementUI;
    [SerializeField] BackgroundMusicController bgmController;

    //Actions.
    public Action<float> OnStationArriveAction; //arg : predepart까지의 남은 시간.
    public Action OnPreDepartAction;
    public Action OnDangerAction;
    public Action<float> OnStationDepartAction;
    public Action<int> OnMapLoadFinishingAction; //맵 로딩 완료시 매니저에서 한번 더 호출.Arg는 mapIndex.





    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        trainController = FindAnyObjectByType<TrainController>();
        runtimeDungeon = FindAnyObjectByType<RuntimeDungeon>();
        gamePlayManagementUI = FindAnyObjectByType<GamePlayManagementUI>();
        bgmController = GetComponentInChildren<BackgroundMusicController>();
        runtimeDungeon.Generator.OnGenerationComplete += ChangeIsMapReady;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += EnableTrainDepartable;
        newMapReady = false;

        //선로에서 시작. 맵 로딩.
        GoWaitingState();
    }

    private void ChangeIsMapReady(DungeonGenerator generator)
    {
        newMapReady = true; // 맵 생성 완료

        // 배열을 변수에 담지 않고 바로 순회
        foreach (var spawner in FindObjectsByType<GunSpawner>(FindObjectsSortMode.None))
            spawner.InitializeGunProp(currentMapIndex);

        //맵 로딩 완료시 할 일 Invoke.
        OnMapLoadFinishingAction?.Invoke(currentMapIndex);
    }

    #region GameState Logics

    /// 선로구역으로 전송하기.
    public async void GoWaitingState()
    {
        //상태변화 시작
        currentGameState = GameState.Waiting;
        newMapReady = false;

        nextWaitingEndtime = Timer + waitingDuration;

        //대기 레일로 보내기.
        trainController.MoveToWaitingRail();

        //생성된 적 제거.
        EnemyPoolManager.Instance.ReturnAllEnemiesToPool();

        //다음맵 로딩 시작
        currentMapIndex++;
        await MapGenerationManager.Instance.LoadMap(currentMapIndex);

        OnStationDepartAction?.Invoke(waitingDuration);

        DisableTrainDepartable(); //열차 출발 막기. TODO : 시작을 이곳에서 하지 않으면 필요 없음.
    }

    private void GoStageEnteringState()
    {
        currentGameState = GameState.Entering;

        trainController.MoveToStageRail();
    }

    /// <summary>
    /// 기차 도착, 문열고 전투 개시
    /// </summary>
    public void GoCombatState()
    {
        currentGameState = GameState.Combat;
        nextNormalCombatEndTime = Timer + normalCombatDuration;

        trainController.TrainArrive();

        bgmController.PlayRandomCombatMusic();

        Debug.Log("기차가 역에 도착");
        OnStationArriveAction?.Invoke(normalCombatDuration);
    }

    public void GoPreDepartingState()
    {
        currentGameState = GameState.PreDeparting;

        //플레이어가 진입한 것 까지 확인. 문 닫음.
        trainController.DoorClose();

        OnPreDepartAction?.Invoke();

        StartCoroutine(PreDepartingCoroutine());
    }

    public void GoDangerState()
    {
        currentGameState = GameState.Danger;

        OnDangerAction?.Invoke();

        StartCoroutine(DangerDepartingCoroutine());
    }

    IEnumerator PreDepartingCoroutine()
    {
        // 문 닫고, 내부 적이 전부 사라질 때까지 대기
        yield return new WaitUntil(() => !trainController.CheckEnemyInside());

        GoCombatEndState();
    }


    IEnumerator DangerDepartingCoroutine()
    {
        // 플레이어가 탑승할 때까지 대기
        yield return new WaitUntil(() => trainController.CheckPlayerInside() && trainDepartability);

        GoPreDepartingState();
    }


    /// <summary>
    /// 전투시간 종료, 기차 출발준비.
    /// </summary>
    public void GoCombatEndState()
    {
        currentGameState = GameState.Departing;

        DisableTrainDepartable();

        trainController.TrainDepart();

        bgmController.StopCombatMusic();
    }

    #endregion

    #region Fuse Logics

    private void DisableTrainDepartable()
    {
        trainDepartability = false;
    }

    private void EnableTrainDepartable()
    {
        trainDepartability = true;
    }
    [SerializeField] bool trainDepartability = true;

    #endregion

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;

        if (currentGameState == GameState.Waiting && Timer >= nextWaitingEndtime && newMapReady)
        {
            GoStageEnteringState();
        }

        if (currentGameState == GameState.Combat && Timer >= nextNormalCombatEndTime)
        {
            //플레이어가 안에 있고, 출발 가능한 상태. 
            if (trainDepartability && trainController.CheckPlayerInside())
            {
                Debug.Log("플레이어가 안에 있음. PreDepart State");
                //UI는 숨기고, 내부의 적이 다 처리될 때까지 기다림.
                GoPreDepartingState();
            }
            else //플레이어가 도착하지 못함. or 출발불가상태.
            {
                Debug.Log("플레이어가 안에 없음. Danger State");
                GoDangerState();
            }
        }
    }

    void OnDisable()
    {
        runtimeDungeon.Generator.OnGenerationComplete -= ChangeIsMapReady;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= EnableTrainDepartable;
    }
}



