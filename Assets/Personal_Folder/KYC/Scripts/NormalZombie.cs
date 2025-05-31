public class NormalZombie : ZombieBase
{
    protected override void Start()
    {
        health = 100f;
        moveSpeed = 2f;
        base.Start();
    }
}
