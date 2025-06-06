using UnityEngine;

public class AttackState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private bool _isAttacking = false;

    public void Enter(ZombieBase zombie)
    {
        _zombie = zombie;
        _player = GameObject.FindWithTag("Player")?.transform;
        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
            return;
        }
        _zombie.StopMovement();
        StartAttack();
    }

    public void Update()
    {
        if (_isAttacking) return;

        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
            return;
        }

        float distance = Vector3.Distance(_zombie.transform.position, _player.position);
        if (distance <= _zombie.attackRange)
        {
            StartAttack();
        }
        else
        {
            _zombie.SetState(new ChaseState());
        }
    }

    public void Exit()
    {
        _zombie.ResumeMovement();
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _zombie.Animator.SetTrigger("ToAttack");
    }

    public void OnAttackEnd()
    {
        _isAttacking = false;
    }
}