using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Akila.FPSFramework;

public class ChaseState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private float _nextSoundTime = 0f;
    private Coroutine _checkRoutine;
    private Coroutine _chaseRoutine;
    private bool _isChecking = false;
    private FirstPersonController _playerController;

    Vector3 flankOffset;
    float predictionTime;

    private float _randomSpeed;
    private int _predictSkill;
    private float _stupidPercent;

    private Vector3 _lastDestination = Vector3.zero;
    private float _destinationUpdateThreshold = 0.5f;

    private static readonly WaitForSeconds chaseWait = new WaitForSeconds(0.1f);

    public void Enter(ZombieBase zombie)
    {
        _zombie = zombie;
        _player = _zombie.Player;

        _zombie.Animator.SetTrigger("ToChase");
        _zombie.Agent.speed = _zombie.moveSpeed;
        _zombie.SetNotBeDespawned();
        _playerController = _player.GetComponent<FirstPersonController>();


        predictionTime = Random.Range(0.2f, 1.2f);

        if (_zombie.UseRandomSpeed)
        {
            predictionTime = 0.8f;      
            _randomSpeed = Random.Range(4.0f, 7.0f);
            _zombie.moveSpeed = _randomSpeed;
            _zombie.Agent.speed = _randomSpeed;
        }
        else
        {
            _zombie.Agent.speed = _zombie.moveSpeed;
        }




        if (!_isChecking && _player != null)
        {
            _checkRoutine = _zombie.StartCoroutine(CheckPlayerDistance());
            _isChecking = true;
        }

        PlayRandomSound(_zombie.chaseClips);

        _chaseRoutine = _zombie.StartCoroutine(ChaseRoutine());
    }

    public void Update()
    {
        if (_player == null)
        {
            _zombie.SetState(new PatrolState());
        }
    }

    private IEnumerator ChaseRoutine()
    {
        var agent = _zombie.Agent;

        agent.speed = _zombie.moveSpeed;
        agent.acceleration = 25f;
        agent.angularSpeed = 720f;
        agent.updateRotation = true;
        agent.updatePosition = true;

        _zombie.Animator.applyRootMotion = false;

        while (true)
        {
            if (_player == null || _playerController == null || !agent.isOnNavMesh)
                yield break;

            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                    yield return null;
                }
                else
                {
                    yield return chaseWait;
                    continue;
                }
            }

            Vector3 toZombie = (_zombie.transform.position - _player.position).normalized;
            float angle = Vector3.Angle(_player.forward, toZombie);
            float distance = Vector3.Distance(_zombie.transform.position, _player.position);

            Vector3 playerVelocity = _playerController.Velocity;
            Vector3 predictedPosition = (playerVelocity.magnitude < 1f)
                ? _player.position
                : _player.position + playerVelocity * predictionTime;

            Vector3 target;

            if (angle < 20f)
            {
                target = (distance < 20f) ? _player.position : predictedPosition;
            }
            else if (angle < 120f)
            {
                if (distance >= 10f)
                {
                    Vector3 forward = _player.forward;
                    Vector3 side = Vector3.Cross(Vector3.up, forward).normalized;
                    int direction = Vector3.Dot(side, (_zombie.transform.position - _player.position)) > 0 ? 1 : -1;

                    flankOffset = side * 5f * direction - forward * 3f;
                    target = predictedPosition + flankOffset;
                }
                else if (distance < 10f)
                {
                    target = _player.position;
                }
                else
                {
                    target = _player.position + flankOffset;
                }
            }
            else
            {
                target = _player.position + _player.forward * 5f;
            }

            SafeSetDestination(target);

            yield return chaseWait;
        }
    }

    private void SafeSetDestination(Vector3 target)
    {
        if (_zombie.Agent == null || !_zombie.Agent.isOnNavMesh) return;

        if (Vector3.Distance(target, _lastDestination) < _destinationUpdateThreshold)
            return;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            _zombie.Agent.SetDestination(hit.position);
            _lastDestination = hit.position;
        }
        else
        {
            Debug.LogWarning($"{_zombie.name} - NavMesh에서 유효한 목적지를 찾지 못함: {target}");
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
                _zombie.SetState(new PatrolState());
                yield break;
            }

            float distance = Vector3.Distance(_zombie.transform.position, _player.position);

            if (distance < _zombie.attackRange)
            {
                _zombie.SetState(new AttackState());
                yield break;
            }
            else if (distance > _zombie.detectionRange * 1.5f)
            {
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

        _nextSoundTime = Time.time + Random.Range(2f, 4f);
    }
}
