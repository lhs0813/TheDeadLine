public class LargeZombie : ZombieBase
{
    protected override float DefaultHealth => 500f;
    protected override float DefaultMaxHealth => 500f;

    protected override void Start()
    {
        //health = 500f;
        moveSpeed = 6f;
        agent.speed = moveSpeed;  // 추가!
        base.Start();
    }
}
