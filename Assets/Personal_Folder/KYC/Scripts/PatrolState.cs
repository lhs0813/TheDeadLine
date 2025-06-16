using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class PatrolState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private float _nextSoundTime = 0f;
    private Coroutine _checkRoutine;
    private bool _isChecking = false;

    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        _zombie.Animator.SetTrigger("ToPatrol"); // Blend Tree 상태 전이 트리거
        PlayRandomSound(_zombie.patrolClips);
        _player = _zombie.Player;
        if (_player != null)
        {
            _checkRoutine = _zombie.StartCoroutine(CheckPlayer());
            _isChecking = true;
        }

    }

    public void Update()
    {

    }
    private IEnumerator CheckPlayer()
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

            if (!_zombie.isPreSpawn)
            {
                yield return null;
                _zombie.SetState(new ChaseState());
                yield break;
            }

            if (_zombie.CanSeePlayer(_player) ||
                Vector3.Distance(_zombie.transform.position, _player.position) < _zombie.minAttackStartDistance)
            {
                yield return null;
                _zombie.SetState(new ChaseState());
                yield break;
            }

            yield return wait;
        }
    }

    public void Exit() 
    {
                if (_checkRoutine != null)
            _zombie.StopCoroutine(_checkRoutine);
        _isChecking = false;
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
