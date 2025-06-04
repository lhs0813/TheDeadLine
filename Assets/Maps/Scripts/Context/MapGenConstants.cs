public static class MapGenConstants
{
    //BaseValue -> MapIndex. 

    ///Room Creatures Spawn Constants
    public const int BaseCreatureCountOnSpawnRoom = 0;
    public const int MinCreatureCountOnSpawnRoomMultiplier = 2; //스폰 최소치 배율. stageIndex = 5라면 방마다 최소 10마리 생성.
    public const int MaxCreatureCountOnSpawnRoomMultiplier = 3; //스폰 최대치 배율. stageIndex = 5라면 방마다 최대 15마리까지 생성.
    public const int MaxCreatureCountOnSpawnRoom = 25; //한 방에서 생성될 수 있는 적의 최대 수.


    ///Global SkillEnforcement Spawns
    public const int MaxSkillPointItemCountOnStage = 20; //한 스테이지에서 생성될 수 있는 최대 스킬포인트.
    public const int MaxSkillPointItemCountMultiplier = 2; //stageIndex = 5에서 최대 생성되는 스킬포인트 10개.
    public const int MinSkillPointItemCountMultiplier = 1; //최소 5개.

    ///
    public const int MaxGunPropCountMultiplier = 5; //StageIndex = 5에서 생성되는 최대 총기 개수 25개.
    public const int MinGunPropCountMultiplier = 4; //StageIndex = 4에서 생성되는 최소 총기 개수 20개.

    ///Prop Indice
    public const int GunPropIndex = 0; //Global Prop 컴포넌트에서 사용하는 총기 프롭의 Index.
    public const int SkillPointItemIndex = 1;  //Global Prop 컴포넌트에서 사용하는 스킬포인트 아이템의 Index.

    ///Max Enemy Count On a stage
    public const int MaxNormalCreatureCountLimitOnStage = 250; //스테이지 내 일반 적의 수의 리미트.
    public const int BaseMaxNormalCreatureCountOnStage = 100; //하나의 스테이지에 존재할 수 있는 일반 적의 최대 수.
    public const int MaxNormalCreatureCountOffsetByStage = 5; //스테이지마다 증가하는 존재가능한 일반 적의 최대 수.

    public const int MaxBigCreatureCountLimitOnStage = 30; //스테이지 내 거대 적의 수의 리미트.
    public const int BaseMaxBigCreatureCountOnStage = 10; //하나의 스테이지에 존재할 수 있는 거대 적의 최대 수.
    public const int MaxBigCreatureCountOffsetByStage = 2; //스테이지마다 증가하는 존재가능한 거대 적의 최대 수.

    public const int MaxBombCreatureCountLimitOnStage = 20; //스테이지 내 자폭형 적의 수의 리미트.
    public const int BaseMaxBombCreatureCountOnStage = 5; //하나의 스테이지에 존재할 수 있는 자폭형 적의 최대 수.
    public const int MaxBombCreatureCountOffsetByStage = 1; //스테이지마다 증가하는 존재가능한 자폭형 적의 최대 수.

    ///Enemy Spawn Reference - Constants.
    public const float MaxCreatureSpawnRadius = 30f;
    public const float MinCreatureSpawnRadius = 15f;
    public const float BaseEnemyTriggerCooldown = 10f;
    public const float EnemyTriggerCooldownOffset = -0.1f;
    public const float EnemyTriggerCooldownLimit = 5f;
    public const float EnemyTriggerAngleFromPlayer = 90f;

}
