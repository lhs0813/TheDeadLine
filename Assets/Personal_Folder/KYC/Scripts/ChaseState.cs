using UnityEngine;
using System.Collections;

public class ChaseState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private float _nextSoundTime = 0f;
    private Coroutine _checkRoutine;
    private Coroutine _chaseRoutine;          // 새로 추가
    private bool _isChecking = false;
    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _player = _zombie.Player;
        _zombie.Animator.SetTrigger("ToChase"); // Blend Tree 상태 전이 트리거
        _zombie.Agent.speed = _zombie.moveSpeed; // 또는 원하는 값 (예: 4f)
        _zombie.SetNotBeDespawned(); //시스템에 의해 회수되지 않도록 수정.
        if (!_isChecking && _player != null)
        {
            _checkRoutine = _zombie.StartCoroutine(CheckPlayerDistance());
            _isChecking = true;
        }
        PlayRandomSound(_zombie.chaseClips);

        //매 0.2초마다 경로 업데이트
        _chaseRoutine = _zombie.StartCoroutine(ChaseRoutine());
    }

    public void Update()
    {
        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
            return;
        }
    }

    private IEnumerator ChaseRoutine()
    {
        var wait = new WaitForSeconds(0.4f);
        while (true)
        {
            if (_player == null) yield break;
            _zombie.Agent.SetDestination(_player.position);
            yield return wait;
        }
    }

    public void Exit()
    {
        if (_checkRoutine != null)
            _zombie.StopCoroutine(_checkRoutine);

        if (_chaseRoutine != null)
            _zombie.StopCoroutine(_chaseRoutine);


        _isChecking = false;
    }
    private IEnumerator CheckPlayerDistance()
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        while (true)
        {
            if (_player == null)
            {
                yield return null;
                _zombie.SetState(new PatrolState());
                yield break;
            }

            float distance = Vector3.Distance(_zombie.transform.position, _player.position);
            if (distance < _zombie.attackRange)
            {
                yield return null;
                _zombie.SetState(new AttackState());
                yield break;
            }
            else if (distance > _zombie.detectionRange * 1.5f)
            {
                yield return null;
                _zombie.SetState(new PatrolState());
                yield break;
            }

            yield return wait;
        }
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
