public class LargeZombie : ZombieBase
{
    protected override float DefaultHealth => 500f;
    protected override float DefaultMaxHealth => 500f;

    public override bool UseRandomSpeed => false;

    protected override void Start()
    {
        agent.speed = moveSpeed;
        base.Start();
    }

    protected override void OnEnable()
    {
        maxHealth = EnemyConstants.big_baseHP + EnemyConstants.big_0ffset * GamePlayManager.instance.currentMapIndex;
        base.OnEnable();
    }
}
