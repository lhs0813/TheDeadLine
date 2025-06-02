using UnityEngine;

public class DeadState : IZombieState
{
    private ZombieBase _zombie;
    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        Debug.Log($"{zombie.name} 사망 상태 시작");
        _zombie.Animator.SetTrigger("TriggerDeath");

    }

    public void Update() { }

    public void Exit() { }
}
