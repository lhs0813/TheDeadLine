using UnityEngine;

public class AttackState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private bool _isAttacking = false;
    private float _nextSoundTime = 0f;

    public void Enter(ZombieBase zombie)
    {
        _zombie = zombie;
        _player = _zombie.Player;
        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
            return;
        }
        PlayRandomSound(_zombie.attackClips, loop: false);
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