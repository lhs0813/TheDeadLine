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
        PlayRandomSound(_zombie.patrolClips);
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    public void Update()
    {
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
