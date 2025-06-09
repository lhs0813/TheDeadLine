using UnityEngine;

public class PatrolState : IZombieState
{
    private ZombieBase _zombie;
    private Vector3 _patrolTarget;
    private Transform _player;
    private float _nextSoundTime = 0f;
    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _zombie.Animator.SetTrigger("ToPatrol"); // Blend Tree 상태 전이 트리거
        _patrolTarget = GetRandomPoint(); 
        zombie.MoveTowards(_patrolTarget);
        _zombie.Agent.speed = _zombie.moveSpeed;
        PlayRandomSound(_zombie.patrolClips);
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


    private void PlayRandomSound(AudioClip[] clips, bool loop = true)
    {
        if (Time.time < _nextSoundTime) return;

        if (clips == null || clips.Length == 0 || _zombie.audioSource == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        _zombie.audioSource.clip = clip;
        _zombie.audioSource.loop = loop;
        _zombie.audioSource.Play();

        _nextSoundTime = Time.time + Random.Range(2f, 4f); // 쿨타임 부여
    }

}
