using UnityEngine;
using Akila.FPSFramework;
using UnityEngine.AI;

public class DeadState : IZombieState
{
    private float _nextSoundTime = 0f;
    private ZombieBase _zombie;
    public void Enter(ZombieBase zombie)
    {
        this._zombie = zombie;
        Debug.Log($"{zombie.name} 사망 상태 시작");
        _zombie.Animator.SetTrigger("ToDeath");
        _zombie.StopMovement();
        PlayRandomSound(_zombie.deathClips, loop: false);
        if(SkillEffectHandler.Instance.absorbHeatlh)
        {
            // 플레이어 오브젝트 확인
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                // Damageable 컴포넌트 가져오기
                var damageable = playerObj.GetComponent<Damageable>();
                if (damageable != null)
                {
                    damageable.health += 1;
                    Player_Manager.PlayerHpChange?.Invoke(damageable.health);
                }
            }
        }

    }

    public void Update() { }

    public void Exit() { }

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
