public class FastZombie : ZombieBase
{
    protected override void Start()
    {
        health = 100f;
        moveSpeed = 0.3f;
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }
}
