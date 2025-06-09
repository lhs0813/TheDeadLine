public class NormalZombie : ZombieBase
{
    protected override void Start()
    {
        health = 100f;
        moveSpeed = 5f;
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }
}
