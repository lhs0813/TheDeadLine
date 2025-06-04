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
        await InitializeNormalCreatures();
    }

    public async Task InitializeEnemyPools(Dictionary<EnemyType, int> counts)
    {
        if (normalCreaturePrefabs.Count == 0)
            await InitializeNormalCreatures();

        await InitializeBigCreature();
        await InitializeBombCreature();

        enemyPools[EnemyType.Normal] = new ObjectPool<GameObject>(
            CreateNormalCreature,
            GetNormalCreature,
            ReleaseNormalCreature,
            DestroyNormalCreature,
            defaultCapacity: counts[EnemyType.Normal],
            maxSize: counts[EnemyType.Normal] * 2
        );

        enemyPools[EnemyType.Big] = new ObjectPool<GameObject>(
            CreateBigCreature,
            GetBigCreature,
            ReleaseBigCreature,
            DestroyBigCreature,
            defaultCapacity: counts[EnemyType.Big],
            maxSize: counts[EnemyType.Big] * 2
        );

        enemyPools[EnemyType.Bomb] = new ObjectPool<GameObject>(
            CreateBombCreature,
            GetBombCreature,
            ReleaseBombCreature,
            DestroyBombCreature,
            defaultCapacity: counts[EnemyType.Bomb],
            maxSize: counts[EnemyType.Bomb] * 2
        );
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
        return Instantiate(normalCreaturePrefabs[idx], EnemyContainer);
    }

    private void GetNormalCreature(GameObject obj) => obj.SetActive(true);
    private void ReleaseNormalCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyNormalCreature(GameObject obj) => Destroy(obj);

    #endregion

    #region ObjectPool Arguments - Big Creature
    private async Task InitializeBigCreature()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy_Big");
        bigCreaturePrefab = await handle.Task;
    }

    private GameObject CreateBigCreature() => Instantiate(bigCreaturePrefab, EnemyContainer);
    private void GetBigCreature(GameObject obj) => obj.SetActive(true);
    private void ReleaseBigCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyBigCreature(GameObject obj) => Destroy(obj);

    #endregion

    #region ObjectPool Arguments - Bomb Creature
    private async Task InitializeBombCreature()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>("Enemy_Bomb");
        bombCreaturePrefab = await handle.Task;
    }

    private GameObject CreateBombCreature() => Instantiate(bombCreaturePrefab, EnemyContainer);
    private void GetBombCreature(GameObject obj) => obj.SetActive(true);
    private void ReleaseBombCreature(GameObject obj) => obj.SetActive(false);
    private void DestroyBombCreature(GameObject obj) => Destroy(obj);

    #endregion

    #region Common Logics
    public GameObject Spawn(EnemyType type, Vector3 pos, Quaternion rot)
    {
        var obj = enemyPools[type].Get();
        obj.transform.SetPositionAndRotation(pos, rot);
        return obj;
    }

    public void ReturnToPool(EnemyType type, GameObject obj)
    {
        enemyPools[type].Release(obj);
    }

    #endregion
}
