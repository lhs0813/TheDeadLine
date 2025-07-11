using UnityEngine;
using UnityEngine.AI;
using Akila.FPSFramework;
using UnityEngine.PlayerLoop;

public class ChaseState : IZombieState
{
    private ZombieBase _zombie;
    private Transform _player;
    private FirstPersonController _playerController;

    private float _nextSoundTime;
    private float _lastCheckTime;
    private float _lastChaseUpdateTime;

    private float _checkInterval;
    private float _chaseInterval;
    private float _destinationUpdateThreshold = 1.0f;

    private Vector3 _lastDestination = Vector3.zero;
    private Vector3 flankOffset;
    private float predictionTime;

    public void Enter(ZombieBase zombie)
    {
        _zombie = zombie;
        _player = zombie.Player;

        _zombie.Animator.SetTrigger("ToChase");
        _zombie.Agent.speed = _zombie.moveSpeed;
        _zombie.Agent.stoppingDistance = 2.0f;
        _zombie.SetNotBeDespawned();

        _playerController = _player.GetComponent<FirstPersonController>();
        _zombie.Agent.avoidancePriority = Random.Range(20, 80);

        // 초기화
        _lastCheckTime = Time.time;
        _lastChaseUpdateTime = Time.time;

        _checkInterval = Random.Range(0.2f, 0.4f);
        _chaseInterval = Random.Range(0.2f, 0.4f);
        predictionTime = Random.Range(0.45f, 1.0f);

        if (_zombie.UseRandomSpeed)
        {
            predictionTime = 0.8f;
            float randomSpeed = Random.Range(4.0f, 7.0f);
            _zombie.moveSpeed = randomSpeed;
            _zombie.Agent.speed = randomSpeed;
        }

        PlayRandomSound(_zombie.chaseClips);
    }



    public void Update()
    {
        if (_player == null || _playerController == null || !_zombie.Agent.isOnNavMesh)
        {
            _zombie.SetState(new PatrolState());
            return;
        }

        float now = Time.time;

        // 거리 체크
        if (now - _lastCheckTime >= _checkInterval)
        {
            _lastCheckTime = now;
            float distance = Vector3.Distance(_zombie.transform.position, _player.position);

            if (distance < _zombie.attackRange)
            {
                if (_zombie.isBombZombie)
                {
                    _zombie.GetComponent<Explosive>().Damage(100f, _zombie.gameObject, false);
                    _zombie.GetComponent<Damageable>().Damage(100f, _zombie.gameObject, false);
                }
                _zombie.SetState(new AttackState());
                return;
            }
            else if (distance > _zombie.detectionRange * 1.5f)
            {
                _zombie.SetState(new PatrolState());
                return;
            }
        }

        // 타겟 추적 업데이트
        if (now - _lastChaseUpdateTime >= _chaseInterval)
        {
            _lastChaseUpdateTime = now;

            Vector3 toZombie = (_zombie.transform.position - _player.position).normalized;
            float angle = Vector3.Angle(_player.forward, toZombie);
            float distance = Vector3.Distance(_zombie.transform.position, _player.position);

            Vector3 playerVelocity = _playerController.Velocity;
            Vector3 predictedPosition = (playerVelocity.magnitude < 1f)
                ? _player.position
                : _player.position + playerVelocity * predictionTime;

            Vector3 target = CalculateTargetPosition(angle, distance, predictedPosition);
            SafeSetDestination(target);
        }

        // 사운드 재생 간격 조정
        if (Time.time > _nextSoundTime)
        {
            PlayRandomSound(_zombie.chaseClips);
        }
    }

    private Vector3 CalculateTargetPosition(float angle, float distance, Vector3 predictedPosition)
    {
        if (distance > 20f)
            return _player.position;

        if (angle < 20f)
        {
            return (distance < 20f) ? _player.position : predictedPosition;
        }
        else if (angle < 120f)
        {
            if (distance >= 10f)
            {
                Vector3 forward = _player.forward;
                Vector3 side = Vector3.Cross(Vector3.up, forward).normalized;
                int direction = Vector3.Dot(side, (_zombie.transform.position - _player.position)) > 0 ? 1 : -1;

                flankOffset = side * 5f * direction - forward * 3f;
                return predictedPosition + flankOffset;
            }
            else
            {
                return _player.position;
            }
        }
        else
        {
            return _player.position + _player.forward * 5f;
        }
    }

    private void SafeSetDestination(Vector3 target)
    {
        if (_zombie.Agent == null || !_zombie.Agent.isOnNavMesh) return;
        if (Vector3.Distance(target, _lastDestination) < _destinationUpdateThreshold) return;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            _zombie.Agent.SetDestination(hit.position);
            _lastDestination = hit.position;
        }
    }

    private void PlayRandomSound(AudioClip[] clips, bool loop = true)
    {
        if (clips == null || clips.Length == 0 || _zombie.audioSource == null) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        _zombie.audioSource.clip = clip;
        _zombie.audioSource.loop = loop;
        _zombie.audioSource.Play();

        _nextSoundTime = Time.time + Random.Range(3f, 7f);
    }

    public void Exit()
    {
        // 코루틴 없으므로 비워둠
    }

    
}
