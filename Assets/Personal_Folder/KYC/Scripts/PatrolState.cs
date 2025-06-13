using UnityEngine.AI;
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
        // 순찰 목표 도착 시 → 새로운 랜덤 지점 지정
        if (Vector3.Distance(_zombie.transform.position, _patrolTarget) < 1f)
        {
            _patrolTarget = GetRandomPoint();
            _zombie.MoveTowards(_patrolTarget);
        }

        // ⬇️ 조건: 일반 좀비 (isPreSpawn == false) → 무조건 추격
        if (!_zombie.isPreSpawn && _player != null)
        {
            _zombie.SetState(new ChaseState());
        }

        // ⬇️ 조건: 프리스폰 좀비 → 시야에 보이거나, 아주 가까우면 추격
        if (_zombie.isPreSpawn && _player != null)
        {
            if (_zombie.CanSeePlayer(_player) ||
                Vector3.Distance(_zombie.transform.position, _player.position) < _zombie.minAttackStartDistance)
            {
                _zombie.SetState(new ChaseState());
            }
        }
    }


    public void Exit() 
    {
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 origin = _zombie.transform.position;
        Vector2 randomOffset = Random.insideUnitCircle * 5f;
        Vector3 randomPoint = origin + new Vector3(randomOffset.x, 0, randomOffset.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 3f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // 실패했을 경우 현재 위치로 fallback
        return origin;
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
