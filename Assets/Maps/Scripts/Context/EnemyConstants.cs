using System.Text;

public static class EnemyConstants
{
    #region HP points

    // --- Normal Zombie ---
    public const float normal_baseHP = 25f;
    // 9스테이지에서 500이 되도록 미리 계산한 계수
    private static readonly float normal_hpOffset =
        (150f - normal_baseHP) / (9f * 9f);

    // --- Big Zombie ---
    public const float big_baseHP = 400f;
    // 9스테이지에서 3000이 되도록 미리 계산한 계수
    private static readonly float big_hpOffset =
        (900f - big_baseHP) / (9f * 9f);

    // --- Fast Zombie ---
    public const float fast_baseHP = 25f;
    // 9스테이지에서 300이 되도록 미리 계산한 계수
    private static readonly float fast_hpOffset =
        (80f - fast_baseHP) / (9f * 9f);

    // --- Bomb Zombie ---
    public const float bomb_baseHP = 40f;
    // 9스테이지에서 400이 되도록 미리 계산한 계수
    private static readonly float bomb_hpOffset =
        (300f - bomb_baseHP) / (9f * 9f);


    /// <summary>
    /// stage에 따라 HP를 계산합니다.
    /// stage가 9면 각각 500, 1500, 300, 400이고,
    /// 이후에도 stage² 곡선을 따라 무한히 증가합니다.
    /// </summary>
    public static float GetZombieHPByType(EnemyType enemyType, int stage)
    {
        float sq = stage * stage;
        switch (enemyType)
        {
            case EnemyType.Normal:
                return normal_baseHP + normal_hpOffset * sq;
            case EnemyType.Big:
                return big_baseHP + big_hpOffset * sq;
            case EnemyType.Fast:
                return fast_baseHP + fast_hpOffset * sq;
            case EnemyType.Bomb:
                return bomb_baseHP + bomb_hpOffset * sq;
            default:
                return 0f;
        }
    }

    #endregion

    #region Damage (unchanged)

    public const float normal_baseDamage = 5f;
    public const float normal_damageOffset = 1.5f;
    public const float big_baseDamage = 10f;
    public const float big_damageOffset = 3f;
    public const float fast_baseDamage = 5f;
    public const float fast_damageOffset = 1f;
    public const float bomb_baseDamage = 5f;
    public const float bomb_damageOffset = 3f;

    public static float GetZombieDamageByType(EnemyType enemyType, int mapIndex)
    {
        switch (enemyType)
        {
            case EnemyType.Normal:
                return normal_baseDamage + mapIndex * normal_damageOffset;
            case EnemyType.Big:
                return big_baseDamage + mapIndex * big_damageOffset;
            case EnemyType.Fast:
                return fast_baseDamage + mapIndex * fast_damageOffset;
            case EnemyType.Bomb:
                return bomb_baseDamage + mapIndex * bomb_damageOffset;
            default:
                return 0f;
        }
    }

    #endregion
    
     #region Pretty-Print Methods

    /// <summary>
    /// 주어진 스테이지의 좀비 HP를 한 줄씩 포맷팅한 문자열로 반환합니다.
    /// 예) "Normal: 125.00\nBig: 640.00\nFast: 100.00\nBomb: 190.00"
    /// </summary>
    public static string GetZombieHPText(int stage)
    {
        var sb = new StringBuilder();
        var order = new[] { EnemyType.Normal, EnemyType.Big, EnemyType.Fast, EnemyType.Bomb };

        foreach (var type in order)
        {
            float hp = GetZombieHPByType(type, stage);
            sb.AppendLine($"{type}: {hp:F2}");
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    /// <summary>
    /// 주어진 맵 인덱스의 좀비 데미지를 한 줄씩 포맷팅한 문자열로 반환합니다.
    /// 예) "Normal: 15.00\nBig: 35.00\nFast: 8.00\nBomb: 14.00"
    /// </summary>
    public static string GetZombieDamageText(int mapIndex)
    {
        var sb = new StringBuilder();
        var order = new[] { EnemyType.Normal, EnemyType.Big, EnemyType.Fast, EnemyType.Bomb };

        foreach (var type in order)
        {
            float dmg = GetZombieDamageByType(type, mapIndex);
            sb.AppendLine($"{type}: {dmg:F2}");
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    #endregion
}
