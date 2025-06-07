public class NormalZombie : ZombieBase
{
    protected override void Start()
    {
        health = 100f;
        moveSpeed = 5f;
        base.Start();
    }
}
