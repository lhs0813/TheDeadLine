using Akila.FPSFramework;

public class ZombieBomb : ZombieBase
{
    protected override float DefaultHealth => 100f;
    protected override float DefaultMaxHealth => 100f;

    public override bool UseRandomSpeed => false;
    public override bool isBombZombie => true;

    protected override void Start()
    {
        maxHealth = DefaultHealth;
        GetComponent<Explosive>().health = GetComponent<Damageable>().health;
        agent.speed = moveSpeed;
        
        base.Start();
    }

    protected override void OnEnable()
    {
        maxHealth = DefaultHealth;
        GetComponent<Explosive>().health = GetComponent<Damageable>().health;
        base.OnEnable();
    }
}
