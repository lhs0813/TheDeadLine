public class FastZombie : ZombieBase
{
    protected override void Start()
    {
        moveSpeed = 10.5f;
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }

    protected override void OnEnable()
    {
        maxHealth = EnemyConstants.fast_baseHP + EnemyConstants.fast_offset * GamePlayManager.instance.currentMapIndex;
        base.OnEnable();
    }

    private void OnDisable()
    {
        transform.localPosition = UnityEngine.Vector3.zero;
    }
}
