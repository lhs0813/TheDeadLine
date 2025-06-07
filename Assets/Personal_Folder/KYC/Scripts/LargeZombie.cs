public class LargeZombie : ZombieBase
{
    protected override void Start()
    {
        health = 500f;
        moveSpeed = 3.5f;
        base.Start();
    }
}
