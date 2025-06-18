using System.Numerics;
using UnityEngine;

public class NormalZombie : ZombieBase
{
    protected override void Start()
    {
        health = 100f;
        moveSpeed = 8f;
        agent.speed = moveSpeed;
        base.Start();
    }

    private void OnDisable()
    {
        transform.localPosition = UnityEngine.Vector3.zero;
    }
}