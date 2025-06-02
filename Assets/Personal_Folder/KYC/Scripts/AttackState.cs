using UnityEngine;
using System.Collections;

public class AttackState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private Coroutine _attackRoutine;
    private bool _isAttacking = false;

    public void Enter(ZombieBase zombie)
    {
        _zombie = zombie;
        _player = GameObject.FindWithTag("Player")?.transform;
        _zombie.Animator.SetTrigger("TriggerAttack");
        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
            return;
        }

        _zombie.StopMovement();
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
        if (distance > 2f)
        {
            Debug.Log("Player too far. Go chase!");
            _zombie.ResumeMovement();
            _zombie.SetState(new ChaseState());
        }
    }

    public void Exit()
    {
        if (_attackRoutine != null)
            _zombie.StopCoroutine(_attackRoutine);

        _isAttacking = false;
    }


}
