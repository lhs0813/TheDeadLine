public class LargeZombie : ZombieBase
{
    protected override void Start()
    {
        health = 500f;
        moveSpeed = 1.5f;
        base.Start();
    }
}
