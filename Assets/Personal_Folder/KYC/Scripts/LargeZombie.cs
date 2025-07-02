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
        maxHealth = EnemyConstants.GetZombieHPByType(EnemyType.Big, GamePlayManager.instance.currentMapIndex);
        base.OnEnable();
    }
}
