using System;
using DunGen;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
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
        runtimeDungeon.Generator.OnGenerationComplete += ChangeIsMapReady;
        newMapReady = false;

        //선로에서 시작.
        GoWaitingState();

        //첫 맵 로딩.
        await MapGenerationManager.Instance.LoadMap(1);


    }

    private void ChangeIsMapReady(DungeonGenerator generator)
    {
        newMapReady = true; //맵 생성 완료. true.
    }




    private void GoWaitingState()
    {
        isWaiting = true;
        newMapReady = false;

        nextWaitingEndtime = Timer + waitingDuration;

        trainController.MoveToWaitingRail();
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

        OnStationArriveAction?.Invoke();
    }

    /// <summary>
    /// 전투시간 종료, 기차 출발준비.
    /// </summary>
    public void GoCombatEndState()
    {
        isCombatting = false;
        trainController.TrainDepart();

        OnStationDepartAction?.Invoke();
    }

    private void Update()
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
    }



}
