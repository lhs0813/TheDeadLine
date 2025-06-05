using DunGen;
using UnityEngine;

public static class MapGenCalculator
{
    #region MapContexts
    public static IntRange GetCreatureSpawnCountRangePerRoom(int stageIndex)
    {
        int min = stageIndex * MapGenConstants.MinCreatureCountOnSpawnRoomMultiplier;
        int max = stageIndex * MapGenConstants.MaxCreatureCountOnSpawnRoomMultiplier;

        int clampedMax = Mathf.Min(max, MapGenConstants.MaxCreatureCountOnSpawnRoom);
        return new IntRange(min, clampedMax + 1);
    }

    public static IntRange GetSkillPointItemCountRange(int stageIndex)
    {
        int min = stageIndex * MapGenConstants.MinSkillPointItemCountMultiplier;
        int max = stageIndex * MapGenConstants.MaxSkillPointItemCountMultiplier;

        return new IntRange(min, max);
    }

    public static IntRange GetGunPropCountRange(int stageIndex)
    {
        int min = stageIndex * MapGenConstants.MinGunPropCountMultiplier;
        int max = Mathf.Min(stageIndex * MapGenConstants.MaxGunPropCountMultiplier, MapGenConstants.MaxSkillPointItemCountOnStage);

        return new IntRange(min, max);
    }

    // public static IntRange GetNormalCreatureSpawnerPropCountRange(int stageIndex)
    // {

    // }
    #endregion

    #region EnemyCounts
    public static int GetMaxNormalCount(int stageIndex)
    {
        return Mathf.Min(MapGenConstants.MaxNormalCreatureCountLimitOnStage, MapGenConstants.BaseMaxNormalCreatureCountOnStage + stageIndex * MapGenConstants.MaxNormalCreatureCountOffsetByStage);
    }

    public static int GetMaxBigCount(int stageIndex)
    {
        return Mathf.Min(MapGenConstants.MaxBigCreatureCountLimitOnStage, MapGenConstants.BaseMaxBigCreatureCountOnStage + stageIndex * MapGenConstants.MaxBigCreatureCountOffsetByStage);
    }

    public static int GetMaxBombCount(int stageIndex)
    {
        return Mathf.Min(MapGenConstants.MaxBombCreatureCountLimitOnStage, MapGenConstants.BaseMaxBombCreatureCountOnStage + stageIndex * MapGenConstants.MaxBombCreatureCountOffsetByStage);
    }
    #endregion

    #region EnemyTrigger
    public static FloatRange GetHordeTriggerCooldownRange(int stageIndex)
    {
        return new FloatRange(MapGenConstants.BaseEnemyTriggerCooldown + stageIndex * MapGenConstants.EnemyTriggerCooldownOffset, MapGenConstants.BaseEnemyTriggerCooldown);
    }

    public static FloatRange GetHordeTriggerRadiusFromPlayerRange()
    {
        return new FloatRange(MapGenConstants.MinCreatureSpawnRadius, MapGenConstants.MaxCreatureSpawnRadius);
    }
    #endregion
}
