using DunGen;
using UnityEngine;

public static class MapGenCalculator
{
    #region MapContexts
    public static IntRange GetCreatureSpawnCountRangePerSpawner(int stageIndex)
    {
        int min = stageIndex * MapGenConstants.MinCreatureCountOnSpawnRoomMultiplier;
        int max = stageIndex * MapGenConstants.MaxCreatureCountOnSpawnRoomMultiplier;

        int clampedMax = Mathf.Min(max, MapGenConstants.MaxCreatureCountOnSpawnRoom);
        return new IntRange(1, clampedMax + 1);
    }


    #endregion

    // #region EnemyCounts
    // public static int GetMaxNormalCount(int stageIndex)
    // {
    //     return Mathf.Min(MapGenConstants.MaxNormalCreatureCountLimitOnStage, MapGenConstants.BaseMaxNormalCreatureCountOnStage + stageIndex * MapGenConstants.MaxNormalCreatureCountOffsetByStage);
    // }

    // public static int GetMaxBigCount(int stageIndex)
    // {
    //     return Mathf.Min(MapGenConstants.MaxBigCreatureCountLimitOnStage, MapGenConstants.BaseMaxBigCreatureCountOnStage + stageIndex * MapGenConstants.MaxBigCreatureCountOffsetByStage);
    // }

    // public static int GetMaxBombCount(int stageIndex)
    // {
    //     return Mathf.Min(MapGenConstants.MaxBombCreatureCountLimitOnStage, MapGenConstants.BaseMaxBombCreatureCountOnStage + stageIndex * MapGenConstants.MaxBombCreatureCountOffsetByStage);
    // }
    // #endregion
}
