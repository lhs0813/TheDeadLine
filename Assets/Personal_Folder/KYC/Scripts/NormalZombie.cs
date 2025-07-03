using System.Numerics;
using UnityEngine;

public class NormalZombie : ZombieBase
{
    protected override void Start()
    {
        moveSpeed = 8f;
        agent.speed = moveSpeed;
        base.Start();
    }

    protected override void OnEnable()
    {
        maxHealth = EnemyConstants.GetZombieHPByType(EnemyType.Normal, GamePlayManager.instance.currentMapIndex);
        base.OnEnable();
    }

    private void OnDisable()
    {
        transform.localPosition = UnityEngine.Vector3.zero;
    }
}