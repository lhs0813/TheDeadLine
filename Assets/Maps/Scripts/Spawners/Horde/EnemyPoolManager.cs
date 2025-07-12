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
    Bomb,
    Fast
}

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    private Dictionary<EnemyType, ObjectPool<GameObject>> enemyPools = new();
    public List<GameObject> activeEnemies = new();

    public Transform EnemyContainer;

    [SerializeField] private List<GameObject> normalCreaturePrefabs;

    private GameObject bigCreaturePrefab;
    private GameObject bombCreaturePrefab;
    private GameObject fastCreaturePrefab;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private async void Start()
    {
        InitializeSpawnLimits();
        await InitializeEnemyPools();
    }

    public async Task InitializeEnemyPools()
    {
        if (normalCreaturePrefabs.Count == 0)
            await InitializeNormalCreatures();

        await InitializeBigCreature();
        await InitializeBombCreature();
        await InitializeFastCreature();

        // 1) 풀 생성
        enemyPools[EnemyType.Normal] = new ObjectPool<GameObject>(
            CreateNormalCreature,
            null,
            ReleaseNormalCreature,
            DestroyNormalCreature,
            defaultCapacity: MapGenConstants.MaxNormalCreatureCountLimitOnStage,
            maxSize: MapGenConstants.MaxNormalCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Big] = new ObjectPool<GameObject>(
            CreateBigCreature,
            null,
            ReleaseBigCreature,
            DestroyBigCreature,
            defaultCapacity: MapGenConstants.MaxBigCreatureCountLimitOnStage,
            maxSize : MapGenConstants.MaxBigCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Bomb] = new ObjectPool<GameObject>(
            CreateBombCreature,
            null,
            ReleaseBombCreature,
            DestroyBombCreature,
            defaultCapacity: MapGenConstants.MaxBombCreatureCountLimitOnStage,
            maxSize: MapGenConstants.MaxBombCreatureCountLimitOnStage
        );

        enemyPools[EnemyType.Fast] = new ObjectPool<GameObject>(
            CreateFastCreature,
            null,
            ReleaseFastCreature,
            DestroyFastCreature,
            defaultCapacity: MapGenConstants.MaxFastCreatureCountLimitOnStage,
            maxSize: MapGenConstants.MaxFastCreatureCountLimitOnStage
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

        // 4) Fast 타입 풀 채우기
        int fastWarmCount = MapGenConstants.MaxFastCreatureCountLimitOnStage;
        var fastPool = enemyPools[EnemyType.Fast];
        for (int i = 0; i < fastWarmCount; i++)
        {
            GameObject newFast = CreateFastCreature();
            fastPool.Release(newFast);
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

    
    private void ReleaseBigCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyBigCreature(GameObject obj) => DestroyImmediate(obj);
    #endregion

    #region ObjectPool Arguments - Bomb Creature
    private async Task InitializeBombCreature()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy_Bomb");
        bombCreaturePrefab = await handle.Task;
    }

    private GameObject CreateBombCreature()
    {
        var newObj = Instantiate(bombCreaturePrefab, Vector3.zero, Quaternion.identity, EnemyContainer);
        newObj.SetActive(false);
        return newObj;
    }

    private void ReleaseBombCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyBombCreature(GameObject obj) => DestroyImmediate(obj);
    #endregion

    #region ObjectPool Arguments - Big Creature
    private async Task InitializeFastCreature()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy_Fast");
        fastCreaturePrefab = await handle.Task;
    }

    private GameObject CreateFastCreature()
    {
        var newObj = Instantiate(fastCreaturePrefab, Vector3.zero, Quaternion.identity, EnemyContainer);
        newObj.SetActive(false);
        return newObj;
    }

    
    private void ReleaseFastCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyFastCreature(GameObject obj) => DestroyImmediate(obj);
    #endregion

    #region Common Logics
    //0611 김현우 변경 : Spawn 시 Transform의 자식으로 설정. 렌더링 초기화 위함.
    public GameObject Spawn(EnemyType type, Vector3 pos, Quaternion rot, bool isPrespawn)
    {
        if (currentCounts[type] >= maxCounts[type])
            return null;

        var obj = enemyPools[type].Get(); //적 object 가져오기. Normal형 적의 경우에는 Random 포함된 구조.

        if (obj == null)
        {
            Debug.Log("스폰시킬 적을 가져오지 못함");
            return null;
        }

        if (obj.TryGetComponent<EnemyIdentifier>(out var id))
            {
                if (id != null)
                {
                    id.Type = type; //타입 설정 (pool 전용.)
                    id.isPrespawn = isPrespawn; //선제 스폰 적인지 구분.                
                }

            }

        //위치 설정 후 SetActive.True
        obj.transform.SetPositionAndRotation(pos, rot);
        //obj.transform.SetParent(spawnerTransform);
        obj.SetActive(true);

        currentCounts[type]++;
        activeEnemies.Add(obj);
        return obj;
    }


    public void ReturnToPool(EnemyType type, GameObject obj, float offset)
    {
        // 1) Guard: if this enemy isn't marked active, don't schedule a return at all.
        if (!activeEnemies.Contains(obj))
            return;

        StartCoroutine(CorpseDisappearCoroutine(type, obj, offset));
    }

IEnumerator CorpseDisappearCoroutine(EnemyType type, GameObject obj, float offset)
{
    yield return new WaitForSeconds(offset);

        // 2) Double‐check: maybe it was removed by something else in the meantime?
        if (!activeEnemies.Contains(obj))
        {
            //DestroyImmediate(obj);
            yield break;
        }


    // reset position (optional)
    obj.transform.position = Vector3.zero;

    // decrement count safely
    currentCounts[type] = Mathf.Max(0, currentCounts[type] - 1);

    // actually release back to the pool
    enemyPools[type].Release(obj);

    // remove from active list
    activeEnemies.Remove(obj);
}

    //public void ReturnAllEnemiesToPool()
    //{
    //    //적 사망 반환 코루틴 진행중인 경우 취소.
    //    StopAllCoroutines();

    //    //배열 내 생성되어있는 모든 적들 반환.
    //    for (int i = activeEnemies.Count - 1; i >= 0; i--)
    //    {
    //        Debug.Log("생성된 적 반환");

    //        GameObject enemy = activeEnemies[i];
    //        if (enemy != null)
    //        {
    //            EnemyType type = enemy.GetComponent<EnemyIdentifier>().Type;
    //            ReturnToPool(type, enemy, 0f);
    //        }
    //    }

    //    //배열 Clear.
    //    activeEnemies.Clear();
    //}

    public void ReturnAllEnemiesToPool()
    {
        // 1) 진행 중인 코루틴 취소
        StopAllCoroutines();
        // 2) 리스트에 있는 모든 적을 즉시 풀에 반환
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null)
                continue;
            // 타입 가져오기
            var type = enemy.GetComponent<EnemyIdentifier>().Type;
            // (옵션) 위치 초기화
            enemy.transform.position = Vector3.zero;
            // 카운트 감소
            currentCounts[type] = Mathf.Max(0, currentCounts[type] - 1);
            // 바로 릴리즈 → actionOnRelease 콜백에서 SetActive(false) 실행
            enemyPools[type].Release(enemy);
        }
        // 3) 액티브 리스트 완전 초기화
        activeEnemies.Clear();
    }

    private Dictionary<EnemyType, int> currentCounts = new();
    private Dictionary<EnemyType, int> maxCounts = new();

    public void InitializeSpawnLimits()
    {
        currentCounts[EnemyType.Normal] = 0;
        currentCounts[EnemyType.Big]    = 0;
        currentCounts[EnemyType.Bomb]   = 0;
        currentCounts[EnemyType.Fast]   = 0;      // ← 추가

        maxCounts[EnemyType.Normal] = MapGenConstants.MaxNormalCreatureCountLimitOnStage;
        maxCounts[EnemyType.Big]    = MapGenConstants.MaxBigCreatureCountLimitOnStage;
        maxCounts[EnemyType.Bomb]   = MapGenConstants.MaxBombCreatureCountLimitOnStage;
        maxCounts[EnemyType.Fast]   = MapGenConstants.MaxFastCreatureCountLimitOnStage;  // ← 추가
    }

    #endregion
}
