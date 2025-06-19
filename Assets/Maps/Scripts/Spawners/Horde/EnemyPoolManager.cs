using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public enum EnemyType
{
    Normal,
    Big,
    Bomb
}

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    private Dictionary<EnemyType, ObjectPool<GameObject>> enemyPools = new();
    private List<GameObject> activeEnemies = new();

    public Transform EnemyContainer;

    [SerializeField] private List<GameObject> normalCreaturePrefabs;

    private GameObject bigCreaturePrefab;
    private GameObject bombCreaturePrefab;
    [SerializeField] private float corpseDisappearDuration;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        InitializeSpawnLimits(1);
        await InitializeEnemyPools();
    }

    public async Task InitializeEnemyPools()
    {
        if (normalCreaturePrefabs.Count == 0)
            await InitializeNormalCreatures();

        await InitializeBigCreature();
        await InitializeBombCreature();

        // 1) 풀 생성
        enemyPools[EnemyType.Normal] = new ObjectPool<GameObject>(
            CreateNormalCreature,
            null,
            ReleaseNormalCreature,
            DestroyNormalCreature,
            defaultCapacity: MapGenConstants.MaxNormalCreatureCountLimitOnStage / 2,
            maxSize: MapGenConstants.MaxNormalCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Big] = new ObjectPool<GameObject>(
            CreateBigCreature,
            null,
            ReleaseBigCreature,
            DestroyBigCreature,
            defaultCapacity: MapGenConstants.MaxBigCreatureCountLimitOnStage / 2,
            maxSize: MapGenConstants.MaxBigCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Bomb] = new ObjectPool<GameObject>(
            CreateBombCreature,
            null,
            ReleaseBombCreature,
            DestroyBombCreature,
            defaultCapacity: MapGenConstants.MaxBombCreatureCountLimitOnStage / 2,
            maxSize: MapGenConstants.MaxBombCreatureCountLimitOnStage
        );

        // 2) 풀을 미리 채워두기 (Create → Release)
        InitializeEnemyPoolsObjects();
    }

    /// <summary>
    /// ObjectPool을 defaultCapacity만큼 채우기 위해,
    /// CreateXxxCreature()를 직접 호출하고 곧바로 Release()해서
    /// 풀 안에 서로 다른 인스턴스들을 쌓는 메서드
    /// </summary>
    private void InitializeEnemyPoolsObjects()
    {
        // 1) Normal 타입 풀 채우기
        int normalWarmCount = MapGenConstants.MaxNormalCreatureCountLimitOnStage;

        var normalPool = enemyPools[EnemyType.Normal];
        for (int i = 0; i < normalWarmCount; i++)
        {
            // Get()가 아닌, CreateNormalCreature()를 직접 호출 → 새 인스턴스 생성
            GameObject newNormal = CreateNormalCreature();
            // 풀에 바로 등록 (Release 하면 SetActive(false) 실행되고 푸시됨)
            normalPool.Release(newNormal);
        }

        // 2) Big 타입 풀 채우기
        int bigWarmCount = MapGenConstants.MaxBigCreatureCountLimitOnStage;
        var bigPool = enemyPools[EnemyType.Big];
        for (int i = 0; i < bigWarmCount; i++)
        {
            GameObject newBig = CreateBigCreature();
            bigPool.Release(newBig);
        }

        // 3) Bomb 타입 풀 채우기
        int bombWarmCount = MapGenConstants.MaxBombCreatureCountLimitOnStage;
        var bombPool = enemyPools[EnemyType.Bomb];
        for (int i = 0; i < bombWarmCount; i++)
        {
            GameObject newBomb = CreateBombCreature();
            bombPool.Release(newBomb);
        }
    }

    #region ObjectPool Arguments - Normal Creature
    private async Task InitializeNormalCreatures()
    {
        var handle = Addressables.LoadAssetsAsync<GameObject>("Enemy_Normal", null);
        var prefabs = await handle.Task;
        normalCreaturePrefabs = new List<GameObject>(prefabs);
    }

    private GameObject CreateNormalCreature()
    {
        int idx = UnityEngine.Random.Range(0, normalCreaturePrefabs.Count);
        var newObj = Instantiate(normalCreaturePrefabs[idx], Vector3.zero, Quaternion.identity, EnemyContainer);
        newObj.SetActive(false);
        return newObj;
    }

    private void GetNormalCreature(GameObject obj) => obj.SetActive(true);
    private void ReleaseNormalCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyNormalCreature(GameObject obj) => DestroyImmediate(obj);
    #endregion

    #region ObjectPool Arguments - Big Creature
    private async Task InitializeBigCreature()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy_Big");
        bigCreaturePrefab = await handle.Task;
    }

    private GameObject CreateBigCreature()
    {
        var newObj = Instantiate(bigCreaturePrefab, Vector3.zero, Quaternion.identity, EnemyContainer);
        newObj.SetActive(false);
        return newObj;
    }

    private void GetBigCreature(GameObject obj) => obj.SetActive(true);
    private void ReleaseBigCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyBigCreature(GameObject obj) => DestroyImmediate(obj);
    #endregion

    #region ObjectPool Arguments - Bomb Creature
    private async Task InitializeBombCreature()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy_Fast");
        bombCreaturePrefab = await handle.Task;
    }

    private GameObject CreateBombCreature()
    {
        var newObj = Instantiate(bombCreaturePrefab, Vector3.zero, Quaternion.identity, EnemyContainer);
        newObj.SetActive(false);
        return newObj;
    }

    private void GetBombCreature(GameObject obj) => obj.SetActive(true);
    private void ReleaseBombCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyBombCreature(GameObject obj) => DestroyImmediate(obj);
    #endregion

    #region Common Logics
    //0611 김현우 변경 : Spawn 시 Transform의 자식으로 설정. 렌더링 초기화 위함.
    public GameObject Spawn(EnemyType type, Vector3 pos, Quaternion rot, bool isPrespawn)
    {
        if (currentCounts[type] >= maxCounts[type])
            return null;

        var obj = enemyPools[type].Get(); //적 object 가져오기. Normal형 적의 경우에는 Random 포함된 구조.

        if (obj.TryGetComponent<EnemyIdentifier>(out var id))
        {
            id.Type = type; //타입 설정 (pool 전용.)
            id.isPrespawn = isPrespawn; //선제 스폰 적인지 구분.
        }

        //위치 설정 후 SetActive.True
        obj.transform.SetPositionAndRotation(pos, rot);
        //obj.transform.SetParent(spawnerTransform);
        obj.SetActive(true);

        // 여기서 타입 정보를 설정



        currentCounts[type]++;
        activeEnemies.Add(obj);
        return obj;
    }


    public void ReturnToPool(EnemyType type, GameObject obj, float offset)
    {
        StartCoroutine(CorpseDisappearCoroutine(type, obj, offset));

    }

    IEnumerator CorpseDisappearCoroutine(EnemyType type, GameObject obj, float offset)
    {
        //시체 사라지는 대기시간... 맵 해제 트리거시에도 남아있음. 
        yield return new WaitForSeconds(offset);
        
        obj.transform.position = Vector3.zero; //위치 초기화.
        currentCounts[type] = Mathf.Max(0, currentCounts[type] - 1); //타입별 개수 감소.
        enemyPools[type].Release(obj); //SetActive.False
        activeEnemies.Remove(obj); //배열에서 제거.
    }
    
    public void ReturnAllEnemiesToPool()
    {
        //적 사망 반환 코루틴 진행중인 경우 취소.
        StopAllCoroutines();

        //배열 내 생성되어있는 모든 적들 반환.
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            Debug.Log("생성된 적 반환");

            GameObject enemy = activeEnemies[i];
            if (enemy != null)
            {
                EnemyType type = enemy.GetComponent<EnemyIdentifier>().Type;
                ReturnToPool(type, enemy, 1f);
            }
        }

        //배열 Clear.
        activeEnemies.Clear();
    }

    private Dictionary<EnemyType, int> currentCounts = new();
    private Dictionary<EnemyType, int> maxCounts = new();

    public void InitializeSpawnLimits(int stageIndex)
    {
        currentCounts[EnemyType.Normal] = 0;
        currentCounts[EnemyType.Big] = 0;
        currentCounts[EnemyType.Bomb] = 0;

        maxCounts[EnemyType.Normal] = MapGenCalculator.GetMaxNormalCount(stageIndex);
        maxCounts[EnemyType.Big] = MapGenCalculator.GetMaxBigCount(stageIndex);
        maxCounts[EnemyType.Bomb] = MapGenCalculator.GetMaxBombCount(stageIndex);
    }
    #endregion
}
