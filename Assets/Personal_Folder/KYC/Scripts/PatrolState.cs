using UnityEngine;

public class PatrolState : IZombieState
{
    private ZombieBase _zombie;
    private Vector3 _patrolTarget;
    private Transform _player;

    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _zombie.Animator.SetTrigger("TriggerWalk");
        _patrolTarget = GetRandomPoint(); 
        zombie.MoveTowards(_patrolTarget);
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    public void Update()
    {
        if (Vector3.Distance(_zombie.transform.position, _patrolTarget) < 1f)
        {
            _patrolTarget = GetRandomPoint();
            _zombie.MoveTowards(_patrolTarget);
        }
        if (_player != null && _zombie.IsPlayerInRange(_player))
        {
            _zombie.SetState(new ChaseState());
        }
    }

    public void Exit() 
    {
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 origin = _zombie.transform.position;
        Vector2 randomOffset = Random.insideUnitCircle * 5f;
        return origin + new Vector3(randomOffset.x, 0, randomOffset.y);
    }
}
