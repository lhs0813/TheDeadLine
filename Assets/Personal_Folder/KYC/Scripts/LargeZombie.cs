public class LargeZombie : ZombieBase
{
    protected override void Start()
    {
        health = 500f;
        moveSpeed = 1.0f;
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }
}
