using System;
using System.Threading.Tasks;
using DunGen;
using DunGen.Demo;
using DunGen.Graph;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 맵 생성에 필요한 정보를 담는 Container
/// </summary>
public class MapContext
{
    public DungeonFlow DungeonFlow;
    public IntRange CreatureSpawnCountRangePerRoom;
    public IntRange SkillPointItemCountRange;
    public IntRange GunPropCountRange;

    public MapContext(DungeonFlow dungeonFlow, IntRange creatureSpawnCountRangePerRoom,
        IntRange skillPointItemCountRange, IntRange gunPropCountRange)
    {
        this.DungeonFlow = dungeonFlow;
        this.CreatureSpawnCountRangePerRoom = creatureSpawnCountRangePerRoom;
        this.SkillPointItemCountRange = skillPointItemCountRange;
        this.GunPropCountRange = gunPropCountRange;
    }
}

public class MapGenerationManager : MonoBehaviour
{
    public static MapGenerationManager Instance { get; private set; }

    public MapContext currentMapContext;

    RuntimeDungeon runtimeDungeon;

    public Action OnNavMeshBakeAction;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        runtimeDungeon = GetComponent<RuntimeDungeon>();
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        runtimeDungeon.Generator.OnGenerationComplete += BakeNavMeshOnMapLoaded;
    }

    /// <summary>
    /// 스테이지 인덱스를 받아서 맵 생성.
    /// </summary>
    /// <param name="mapIndex"></param>
    public async Task LoadMap(int mapIndex)
    {
        Debug.Log($"Map Generator : {mapIndex}번 맵을 생성.");

        //Dungeon Flow 설정.
        runtimeDungeon.Generator.DungeonFlow = await GetDungeonFlowAsync(mapIndex);

        //맵 생성.
        runtimeDungeon.Generate();
    }

    private static async Task<DungeonFlow> GetDungeonFlowAsync(int mapIndex)
    {
        int ModifiedMapIndex = mapIndex > 10 ? 10 : mapIndex;

        string key = $"DF_Station_{ModifiedMapIndex}";
        var handle = Addressables.LoadAssetAsync<DungeonFlow>(key);
        var flow = await handle.Task;

        if (flow == null)
        {
            Debug.LogError($"DungeonFlow '{key}' 가 없습니다.");
            return null;
        }
        else
        {
            Debug.Log($"DungeonFlow '{key}' 를 적용했습니다");
        }

        // 🔐 안전하게 복사해서 반환
        return ScriptableObject.Instantiate(flow);
    }

    private void BakeNavMeshOnMapLoaded(DungeonGenerator generator)
    {
        generator.Root.GetComponent<NavMeshSurface>().BuildNavMesh();

        //Nav Mesh 베이크 완료 알림.
        OnNavMeshBakeAction?.Invoke();
    }


}
