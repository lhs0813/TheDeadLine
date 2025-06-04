using System;
using System.Threading.Tasks;
using DunGen;
using DunGen.Graph;
using UnityEngine;

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

    public Action<int> OnMapLoadedAction;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        runtimeDungeon = GetComponent<RuntimeDungeon>();

        await LoadMap(1);
    }



    /// <summary>
    /// 스테이지 인덱스를 받아서 맵 생성.
    /// </summary>
    /// <param name="mapIndex"></param>
    public async Task LoadMap(int mapIndex)
    {
        //MapContext 로딩.
        currentMapContext = await MapContextBuilder.BuildAsync(mapIndex);

        //Dungeon Flow 설정.
        runtimeDungeon.Generator.DungeonFlow = currentMapContext.DungeonFlow;
        //Dungeon Flow - Global Props 설정.
        runtimeDungeon.Generator.DungeonFlow.GlobalProps.Add(new DungeonFlow.GlobalPropSettings(0, currentMapContext.GunPropCountRange));
        runtimeDungeon.Generator.DungeonFlow.GlobalProps.Add(new DungeonFlow.GlobalPropSettings(1, currentMapContext.SkillPointItemCountRange));

        //맵 생성.
        runtimeDungeon.Generate();


        //맵 생성시 트리거할 액션 Invoke.
        OnMapLoadedAction?.Invoke(mapIndex);
    }
    

}
