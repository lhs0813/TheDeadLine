using DunGen;
using UnityEngine;

public static class MapGenCalculator
{
    #region MapContexts
    public static IntRange GetCreatureSpawnCountRangePerSpawner(int stageIndex)
    {
        int min = MapGenConstants.BaseCreatureCountOnSpawnRoom + (int)(stageIndex * MapGenConstants.MinCreatureCountOnSpawnRoomMultiplier);
        int max = MapGenConstants.BaseCreatureCountOnSpawnRoom + (int)(MapGenConstants.MaxCreatureCountOnSpawnRoomMultiplier * stageIndex);

        int clampedMin = Mathf.Min(min, MapGenConstants.MaxMinimumCreatureCountOnSpawnRoom);
        int clampedMax = Mathf.Min(max, MapGenConstants.MaxCreatureCountOnSpawnRoom);
        return new IntRange(clampedMin, clampedMax);
    }

    public static int GetModifiedIndex(int mapIndex)
    {
        if (mapIndex <= 10)
            return mapIndex;

        // 11 이상일 때는 3 ~ 11 사이 임의 반환
        return UnityEngine.Random.Range(3, 11);  // 상한 12 미포함 → 최대 11
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
