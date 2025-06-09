using UnityEngine;
using UnityEngine.AI;

public class DeadState : IZombieState
{
    private ZombieBase _zombie;
    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        Debug.Log($"{zombie.name} 사망 상태 시작");
        _zombie.Animator.SetTrigger("ToDeath");
        _zombie.StopMovement();
        

    }

    public void Update() { }

    public void Exit() { }
}
