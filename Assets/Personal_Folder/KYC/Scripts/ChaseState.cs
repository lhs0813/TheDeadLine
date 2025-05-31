using UnityEngine;

public class ChaseState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;

    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _player = GameObject.FindWithTag("Player")?.transform;
        _zombie.Animator.SetTrigger("TriggerRun");
    }

    public void Update()
    {
        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
            return;
        }

        _zombie.MoveTowards(_player.position);

        float distance = Vector3.Distance(_zombie.transform.position, _player.position);

        if (distance < 1.5f)
        {
            _zombie.SetState(new AttackState());
        }
        else if (distance > _zombie.detectionRange * 1.5f)
        {
            _zombie.SetState(new PatrolState()); 
        }
    }

    public void Exit()
    {
    }
}
