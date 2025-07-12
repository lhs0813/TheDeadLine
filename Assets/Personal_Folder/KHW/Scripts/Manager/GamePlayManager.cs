using System;
using System.Collections;
using System.Threading.Tasks;
using Akila.FPSFramework;
using DunGen;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public enum GameState
{
    Waiting,
    Entering,
    Combat,
    Danger,
    PreDeparting,
    Departing
}

[DefaultExecutionOrder(-100)]
public class GamePlayManager : MonoBehaviour
{
    [Header("Current Stage Info && GameState")]
    public int currentMapIndex = 0;
    public StageInfo currentStageInfo;
    public GameState currentGameState;
    public bool isStoryMode;


    [Header("Instances")]
    public static GamePlayManager instance;
    public RuntimeDungeon runtimeDungeon;
    public TrainController trainController;
    [SerializeField] GamePlayManagementUI gamePlayManagementUI;
    public BackgroundMusicController bgmController;

    [Header("State Checker")]
    public static float Timer;
    private float nextWaitingEndtime;
    private float nextNormalCombatEndTime;
    private bool newMapReady; //맵 생성 완료 알림
    [SerializeField] private float waitingDuration = 20f; // 열차 대기 시간
    [SerializeField] private float normalCombatDuration = 180f; //전투시간. 
    [SerializeField] private float remainingTimeAfterTrainAcceleration = 5f;

    [Header("Actions")]
    public Action<float> OnStationArriveAction; //arg : predepart까지의 남은 시간.
    public Action OnPreDepartAction;
    public Action OnDangerAction;
    public Action<float> OnStationDepartAction;
    public Action<int> OnMapLoadFinishingAction; //맵 로딩 완료시 매니저에서 한번 더 호출.Arg는 mapIndex.
    public Action<float> OnTrainAccelerationAction; //열차를 가속시켰을 때 발동됨.

    public Action OnTrainArriveAction;
    public bool goingUp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        OnTrainArriveAction = null;
    }

    async void Start()
    {
        trainController = FindAnyObjectByType<TrainController>();
        runtimeDungeon = FindAnyObjectByType<RuntimeDungeon>();
        gamePlayManagementUI = FindAnyObjectByType<GamePlayManagementUI>();
        bgmController = GetComponentInChildren<BackgroundMusicController>();
        runtimeDungeon.Generator.OnGenerationComplete += ChangeIsMapReady;

        
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction += EnableTrainDepartable;
        FindAnyObjectByType<DungenCharacter>().OnTileChanged += ManagePlayerLocation;

        newMapReady = false;

        // 1) 무기 프리팹 미리 로드
        //currentMapIndex = 0;
        await SpawnedGunBuilder.InitializeAsync();
        currentStageInfo = await GetStageInfoAsync(MapGenCalculator.GetModifiedIndex(currentMapIndex));

        allFuseActivated = false;
    }

    private void ManagePlayerLocation(DungenCharacter character, Tile previousTile, Tile newTile)
    {
        if (!previousTile.IsMainPath() || !newTile.IsMainPath()) return;

        if (newTile.GetDeepness() > previousTile.GetDeepness()) goingUp = true;
        else goingUp = false;
    }

    private void ChangeIsMapReady(DungeonGenerator generator)
    {
        if (FindAnyObjectByType<Dungeon>().BranchPathTiles.Count < 5)
        {
            
        }

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

        if (isStoryMode && currentMapIndex >= 10)
        {
            SceneManager.LoadScene("EndingScene");

            return;
        }

        //맵 정보 저장 및 생성.
            int ModifiedMapIndex = MapGenCalculator.GetModifiedIndex(currentMapIndex);

        currentStageInfo = await GetStageInfoAsync(ModifiedMapIndex);
        await MapGenerationManager.Instance.LoadMap(ModifiedMapIndex);

        //좀비 확률 설정.
        HordeSpawnBuilder.SetSpawnWeights(currentMapIndex);

        OnStationDepartAction?.Invoke(waitingDuration);

        DisableTrainDepartable(); //열차 출발 막기. TODO : 시작을 이곳에서 하지 않으면 필요 없음.
    }

    public void AccelerationControl()
    {
        if (currentGameState != GameState.Waiting) return;

        nextWaitingEndtime = Timer + remainingTimeAfterTrainAcceleration;

        OnTrainAccelerationAction?.Invoke(remainingTimeAfterTrainAcceleration);
    }

    private static async Task<StageInfo> GetStageInfoAsync(int mapIndex)
    {

        string key = $"StageInfo_{mapIndex}";
        var handle = Addressables.LoadAssetAsync<StageInfo>(key);
        var flow = await handle.Task;

        return ScriptableObject.Instantiate(flow);
    }

    private void GoStageEnteringState()
    {
        currentGameState = GameState.Entering;

        trainController.MoveToStageRail();

        OnTrainArriveAction.Invoke();
    }

    /// <summary>
    /// 기차 도착, 문열고 전투 개시
    /// </summary>
    public void GoCombatState()
    {
        currentGameState = GameState.Combat;
        nextNormalCombatEndTime = Timer + currentStageInfo.combatTime;

        trainController.TrainArrive();

        PreSpawnHordeSpawnerController.isAnyRoomTriggered = false;

        bgmController.PlayRandomCombatMusic();

        Debug.Log("기차가 역에 도착");
        OnStationArriveAction?.Invoke(currentStageInfo.combatTime);
    }

    public void GoFirstCombatState()
    {
        currentGameState = GameState.Combat;
        nextNormalCombatEndTime = Timer + currentStageInfo.combatTime;   

        OnStationArriveAction?.Invoke(currentStageInfo.combatTime);     
    }

    public void GoPreDepartingState()
    {
        currentGameState = GameState.PreDeparting;

        //플레이어가 진입한 것 까지 확인. 문 닫음.
        trainController.DoorClose();

        OnPreDepartAction?.Invoke();

        GoCombatEndState();
    }

    public void GoDangerState()
    {
        currentGameState = GameState.Danger;

        bgmController.PlayRandomDangerMusic();

        OnDangerAction?.Invoke();
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
        allFuseActivated = false;
    }

    private void EnableTrainDepartable()
    {
        allFuseActivated = true;
    }
    [SerializeField] bool allFuseActivated = true;

    #endregion

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;

        if (currentGameState == GameState.Waiting && Timer >= nextWaitingEndtime - remainingTimeAfterTrainAcceleration)
        {
            FindAnyObjectByType<TrainAccelerationButton>().DisableAccelerationButton();
        }

        if (currentGameState == GameState.Waiting && Timer >= nextWaitingEndtime && newMapReady)
        {
            GoStageEnteringState();
        }

        if (currentGameState == GameState.Combat && Timer >= nextNormalCombatEndTime)
        {
            GoDangerState();
        }
    }

    public IEnumerator DelayedLoad() // 사망시 메인메뉴 씬으로 돌아가는 시스템 - 이현수
    {
        UIManager.Instance.gameObject.SetActive(false);
        yield return new WaitForSeconds(5.5f); // 3초 대기
        Cursor.lockState = CursorLockMode.None;  // 마우스 잠금 해제
        Cursor.visible = true;                   // 마우스 커서 보이게
        //SceneManager.LoadScene("Main_Menu");
    }

    void OnDisable()
    {
        runtimeDungeon.Generator.OnGenerationComplete -= ChangeIsMapReady;
        ObjectiveManager.instance.OnStartReturnToTheTrainObjectiveAction -= EnableTrainDepartable;
    }

    #region Train Control Logics

    /// <summary>
    /// 플레이어가 탑승할때 조건 확인 후 출발.
    /// </summary>
    public void CheckDepart()
    {
        if (currentGameState == GameState.Danger || currentGameState == GameState.Combat) //전투중.
        {
            //1. 퓨즈 3개되면 즉시 
            if (allFuseActivated)
            {
                GoPreDepartingState();
            }
        }
    }

    #endregion
}



