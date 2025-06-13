using UnityEngine;

public class ChaseState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private float _nextSoundTime = 0f;
    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _player = GameObject.FindWithTag("Player")?.transform;
        _zombie.Animator.SetTrigger("ToChase"); // Blend Tree 상태 전이 트리거
        _zombie.Agent.speed *= 3f; // 또는 원하는 값 (예: 4f)
        PlayRandomSound(_zombie.chaseClips);
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

        if (distance < _zombie.attackRange)
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
        _zombie.Agent.speed /= 3f;
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
