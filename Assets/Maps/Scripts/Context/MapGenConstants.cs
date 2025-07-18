public static class MapGenConstants
{
    //BaseValue -> MapIndex. 

    ///Room Creatures Spawn Constants
    public const int BaseCreatureCountOnSpawnRoom = 4;
    public const float MinCreatureCountOnSpawnRoomMultiplier = 0.5f; //스폰 최소치 배율. stageIndex = 5라면 방마다 최소 10마리 생성.
    public const float MaxCreatureCountOnSpawnRoomMultiplier = 0.8f; //스폰 최대치 배율. stageIndex = 5라면 방마다 최대 15마리까지 생성.
    public const int MaxMinimumCreatureCountOnSpawnRoom = 5;
    public const int MaxCreatureCountOnSpawnRoom = 15; //한 방에서 생성될 수 있는 적의 최대 수.


    ///Max Enemy Count On a stage
    public const int MaxNormalCreatureCountLimitOnStage = 80; //스테이지 내 일반 적의 수의 리미트.

    public const int MaxBigCreatureCountLimitOnStage = 10; //스테이지 내 거대 적의 수의 리미트.

    public const int MaxBombCreatureCountLimitOnStage = 5; //스테이지 내 자폭형 적의 수의 리미트.

    public const int MaxFastCreatureCountLimitOnStage = 10;

}
