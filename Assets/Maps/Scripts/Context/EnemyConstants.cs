public static class EnemyConstants
{
    #region HP points

    public static float normal_baseHP = 25f;
    public static float normal_offset = 25f;
    public static float normal_baseDamage = 5f;
    public static float normal_damageOffset = 5f;

    public static float big_baseHP = 400f;
    public static float big_0ffset = 100f;
    public static float big_baseDamage = 10f;
    public static float big_damageOffset = 10f;

    public static float fast_baseHP = 25f;
    public static float fast_offset = 15f;
    public static float fast_baseDamage = 5f;
    public static float fast_damageOffset = 5f;

    public static float bomb_baseHP = 40f;
    public static float bomb_offset = 30f;
    public static float bomb_baseDamage = 5f;
    public static float bomb_damageOffset = 3f;

    public static float GetZombieDamageByType(EnemyType enemyType, int mapIndex)
    {
        if (enemyType == EnemyType.Normal) return normal_baseDamage + mapIndex * normal_damageOffset;
        if (enemyType == EnemyType.Big) return big_baseDamage + mapIndex * big_damageOffset;
        if (enemyType == EnemyType.Fast) return fast_baseDamage + mapIndex * fast_damageOffset;

        return 0;
    }

    #endregion
}
