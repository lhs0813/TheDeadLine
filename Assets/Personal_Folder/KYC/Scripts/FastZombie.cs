public class FastZombie : ZombieBase
{
    protected override void Start()
    {
        health = 100f;
        moveSpeed = 7f;
        base.Start();
    }
}
