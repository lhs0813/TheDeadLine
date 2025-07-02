public class FastZombie : ZombieBase
{
    protected override float DefaultHealth => 100f;
    protected override float DefaultMaxHealth => 100f;

    public override bool UseRandomSpeed => false;


    protected override void Start()
    {
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }

    protected override void OnEnable()
    {
        maxHealth = EnemyConstants.GetZombieHPByType(EnemyType.Fast, GamePlayManager.instance.currentMapIndex);
        base.OnEnable();
    }

    private void OnDisable()
    {
        transform.localPosition = UnityEngine.Vector3.zero;
    }
}
