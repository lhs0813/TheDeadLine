public class LargeZombie : ZombieBase
{
    protected override float DefaultHealth => 500f;
    protected override float DefaultMaxHealth => 500f;

    protected override void Start()
    {
        moveSpeed = 6f;
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }

    protected override void OnEnable()
    {
        maxHealth = EnemyConstants.big_baseHP + EnemyConstants.big_0ffset * GamePlayManager.instance.currentMapIndex;
        base.OnEnable();
    }
}
