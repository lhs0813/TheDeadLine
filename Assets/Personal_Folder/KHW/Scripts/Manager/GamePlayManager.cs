using System;
using System.Threading.Tasks;
using DunGen;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public int currentMapIndex = 0;

    public static GamePlayManager instance;
    public RuntimeDungeon runtimeDungeon;
    public TrainController trainController;
    public static float Timer;
    private float nextWaitingEndtime;
    private float newCombatEndTime;
    private bool newMapReady; //맵 생성 완료 알림
    private bool isWaiting; //열차 이동중
    private bool isCombatting; //전투중
    [SerializeField] private float waitingDuration = 20f; // 열차 대기 시간
    [SerializeField] private float combatDuration = 180f; //전투시간. 
    [SerializeField] PlayerHordeTrigger playerHordeTrigger;
    [SerializeField] GamePlayManagementUI gamePlayManagementUI;
    [SerializeField] BackgroundMusicController bgmController;

    //Actions.
    public Action OnStationArriveAction;
    public Action OnStationDepartAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    async void Start()
    {
        trainController = FindAnyObjectByType<TrainController>();
        runtimeDungeon = FindAnyObjectByType<RuntimeDungeon>();
        playerHordeTrigger = FindAnyObjectByType<PlayerHordeTrigger>();
        gamePlayManagementUI = FindAnyObjectByType<GamePlayManagementUI>();
        bgmController = GetComponentInChildren<BackgroundMusicController>();
        runtimeDungeon.Generator.OnGenerationComplete += ChangeIsMapReady;
        newMapReady = false;

        //선로에서 시작.
        await GoWaitingState();

        //첫 맵 로딩.
        //await MapGenerationManager.Instance.LoadMap(currentMapIndex);


    }

    private void ChangeIsMapReady(DungeonGenerator generator)
    {
        newMapReady = true; // 맵 생성 완료

        // 배열을 변수에 담지 않고 바로 순회
        foreach (var spawner in FindObjectsByType<GunSpawner>(FindObjectsSortMode.None))
            spawner.InitializeGunProp(currentMapIndex);
    }





    public async Task GoWaitingState()
    {
        //상태변화 시작
        isWaiting = true;
        newMapReady = false;

        nextWaitingEndtime = Timer + waitingDuration;

        //대기 레일로 보내기.
        trainController.MoveToWaitingRail();

        //생성된 적 제거.
        EnemyPoolManager.Instance.ReturnAllEnemiesToPool();

        //다음맵 로딩 시작
        currentMapIndex++;
        await MapGenerationManager.Instance.LoadMap(currentMapIndex);
    }

    private void GoStageEnteringState()
    {
        isWaiting = false;

        trainController.MoveToStageRail();
    }

    /// <summary>
    /// 기차 도착, 문열고 전투 개시
    /// </summary>
    public void GoCombatState()
    {
        isCombatting = true;
        newCombatEndTime = Timer + combatDuration;

        trainController.TrainArrive();

        bgmController.PlayRandomCombatMusic();

        Debug.Log("기차가 역에 도착");
        OnStationArriveAction?.Invoke();
    }

    /// <summary>
    /// 전투시간 종료, 기차 출발준비.
    /// </summary>
    public void GoCombatEndState()
    {
        isCombatting = false;

        trainController.TrainDepart();

        bgmController.StopCombatMusic();


        OnStationDepartAction?.Invoke();
    }

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;

        if (isWaiting && Timer >= nextWaitingEndtime && newMapReady)
        {
            GoStageEnteringState();
        }

        if (isCombatting && Timer >= newCombatEndTime)
        {
            GoCombatEndState();
        }

        gamePlayManagementUI.UpdateRemainingTimeUI(isCombatting, Timer, newCombatEndTime);
    }
}



