using System;
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
            GetNormalCreature,
            ReleaseNormalCreature,
            DestroyNormalCreature,
            defaultCapacity: MapGenConstants.MaxNormalCreatureCountLimitOnStage / 2,
            maxSize: MapGenConstants.MaxNormalCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Big] = new ObjectPool<GameObject>(
            CreateBigCreature,
            GetBigCreature,
            ReleaseBigCreature,
            DestroyBigCreature,
            defaultCapacity: MapGenConstants.MaxBigCreatureCountLimitOnStage / 2,
            maxSize: MapGenConstants.MaxBigCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Bomb] = new ObjectPool<GameObject>(
            CreateBombCreature,
            GetBombCreature,
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
        int normalWarmCount = MapGenConstants.MaxNormalCreatureCountLimitOnStage / 2;
        var normalPool = enemyPools[EnemyType.Normal];
        for (int i = 0; i < normalWarmCount; i++)
        {
            // Get()가 아닌, CreateNormalCreature()를 직접 호출 → 새 인스턴스 생성
            GameObject newNormal = CreateNormalCreature();
            // 풀에 바로 등록 (Release 하면 SetActive(false) 실행되고 푸시됨)
            normalPool.Release(newNormal);
        }

        // 2) Big 타입 풀 채우기
        int bigWarmCount = MapGenConstants.MaxBigCreatureCountLimitOnStage / 2;
        var bigPool = enemyPools[EnemyType.Big];
        for (int i = 0; i < bigWarmCount; i++)
        {
            GameObject newBig = CreateBigCreature();
            bigPool.Release(newBig);
        }

        // 3) Bomb 타입 풀 채우기
        int bombWarmCount = MapGenConstants.MaxBombCreatureCountLimitOnStage / 2;
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
    public GameObject Spawn(EnemyType type, Vector3 pos, Quaternion rot)
    {
        if (currentCounts[type] >= maxCounts[type])
            return null;

        var obj = enemyPools[type].Get();
        obj.transform.SetPositionAndRotation(pos, rot);
        currentCounts[type]++;
        activeEnemies.Add(obj);
        return obj;
    }

    public void ReturnToPool(EnemyType type, GameObject obj)
    {
        currentCounts[type] = Mathf.Max(0, currentCounts[type] - 1);
        enemyPools[type].Release(obj);
        activeEnemies.Remove(obj);
    }

    public void ReturnAllEnemiesToPool()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemies[i];
            if (enemy != null)
            {
                // TODO : 적 타입 가져오기
                // EnemyType type = enemy.GetComponent<EnemyIdentifier>().Type;
                // ReturnToPool(type, enemy);
            }
        }
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
