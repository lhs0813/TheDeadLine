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
    private bool newMapReady; //맵 생성 완료 알림
    private bool isWaiting; //열차 이동중?
    [SerializeField] private float waitingDuration = 20f; // 열차 대기 시간

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

    private void GoCombatState()
    {
        isWaiting = false;

        trainController.MoveToStageRail();
    }

    private void Update()
    {
        Timer += Time.deltaTime;

        if (isWaiting && Timer >= nextWaitingEndtime && newMapReady)
        {
            GoCombatState();
        }
    }



}
