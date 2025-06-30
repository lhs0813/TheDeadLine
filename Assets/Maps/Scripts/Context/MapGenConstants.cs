public static class MapGenConstants
{
    //BaseValue -> MapIndex. 

    ///Room Creatures Spawn Constants
    public const int BaseCreatureCountOnSpawnRoom = 0;
    public const int MinCreatureCountOnSpawnRoomMultiplier = 1; //스폰 최소치 배율. stageIndex = 5라면 방마다 최소 10마리 생성.
    public const int MaxCreatureCountOnSpawnRoomMultiplier = 2; //스폰 최대치 배율. stageIndex = 5라면 방마다 최대 15마리까지 생성.
    public const int MaxCreatureCountOnSpawnRoom = 4; //한 방에서 생성될 수 있는 적의 최대 수.


    ///Max Enemy Count On a stage
    public const int MaxNormalCreatureCountLimitOnStage = 100; //스테이지 내 일반 적의 수의 리미트.
    public const int BaseMaxNormalCreatureCountOnStage = 80; //하나의 스테이지에 존재할 수 있는 일반 적의 최대 수.
    public const int MaxNormalCreatureCountOffsetByStage = 2; //스테이지마다 증가하는 존재가능한 일반 적의 최대 수.

    public const int MaxBigCreatureCountLimitOnStage = 30; //스테이지 내 거대 적의 수의 리미트.
    public const int BaseMaxBigCreatureCountOnStage = 10; //하나의 스테이지에 존재할 수 있는 거대 적의 최대 수.
    public const int MaxBigCreatureCountOffsetByStage = 2; //스테이지마다 증가하는 존재가능한 거대 적의 최대 수.

    public const int MaxBombCreatureCountLimitOnStage = 30; //스테이지 내 자폭형 적의 수의 리미트.
    public const int BaseMaxBombCreatureCountOnStage = 10; //하나의 스테이지에 존재할 수 있는 자폭형 적의 최대 수.
    public const int MaxBombCreatureCountOffsetByStage = 2; //스테이지마다 증가하는 존재가능한 자폭형 적의 최대 수.

}
