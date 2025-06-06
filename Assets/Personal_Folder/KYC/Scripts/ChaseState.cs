using UnityEngine;

public class ChaseState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;

    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _player = GameObject.FindWithTag("Player")?.transform;
        _zombie.Animator.SetTrigger("ToChase"); // Blend Tree 상태 전이 트리거
        _zombie.Agent.speed = _zombie.moveSpeed * 1.5f; // 또는 원하는 값 (예: 4f)
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
        _zombie.Agent.speed = _zombie.moveSpeed;
    }
}
