public class LargeZombie : ZombieBase
{
    protected override void Start()
    {
        health = 250f;
        moveSpeed = 1f;
        base.Start();
    }
}
