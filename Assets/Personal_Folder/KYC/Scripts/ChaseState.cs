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

    public void Enter(ZombieBase zombie)
    {
        _zombie = zombie;
        _player = _zombie.Player;

        _zombie.Animator.SetTrigger("ToChase");
        _zombie.Agent.speed = _zombie.moveSpeed;
        _zombie.SetNotBeDespawned();
        _playerController = _player.GetComponent<FirstPersonController>();


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
        var wait = new WaitForSeconds(0.1f); // 반응 개선

        var agent = _zombie.Agent;

        // NavMeshAgent 성능 향상 설정
        agent.speed = _zombie.moveSpeed;
        agent.acceleration = 25f;
        agent.angularSpeed = 720f;
        agent.updateRotation = true;
        agent.updatePosition = true;

        // 루트모션이 AI 이동에 방해될 경우 비활성화 필요
        _zombie.Animator.applyRootMotion = false;

        while (true)
        {
            if (_player == null || _playerController == null || !agent.isOnNavMesh) yield break;



            if (!agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
                else
                {
                    yield return wait;
                    continue;
                }
            }

            Vector3 toZombie = (_zombie.transform.position - _player.position).normalized;
            float angle = Vector3.Angle(_player.forward, toZombie);
            float distance = Vector3.Distance(_zombie.transform.position, _player.position);

            // 예측 위치 계산 (속도 없으면 현재 위치 그대로)
            Vector3 playerVelocity = _playerController.Velocity;
            float predictionTime = 0.8f;
            Vector3 predictedPosition = (playerVelocity.magnitude < 1f)
                ? _player.position
                : _player.position + playerVelocity * predictionTime;

            // 추격 로직
            if (angle < 20f)
            {
                // 정면
                agent.SetDestination(distance < 20f ? _player.position : predictedPosition);

            }
            else if (angle < 240f)
            {
                // 측면
                if (distance >= 10f)
                {
                    Vector3 forward = _player.forward;
                    Vector3 side = Vector3.Cross(Vector3.up, forward).normalized;
                    int direction = Vector3.Dot(side, (_zombie.transform.position - _player.position)) > 0 ? 1 : -1;

                    Vector3 flankOffset = side * 5f * direction - forward * 3f;
                    Vector3 flankTarget = predictedPosition + flankOffset;

                    agent.SetDestination(flankTarget);
                }
                else if (distance < 3f)
                {
                    agent.SetDestination(_player.position);
                }
                else
                {
                    agent.SetDestination(predictedPosition);
                }
            }
            else
            {
                // 후방
                Vector3 futurePos = _player.position + _player.forward * 4f;
                agent.SetDestination(futurePos);
            }

            // 좀비가 목적지에 도달했는데 멈췄을 때 재지정
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                agent.SetDestination(_player.position);
            }

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