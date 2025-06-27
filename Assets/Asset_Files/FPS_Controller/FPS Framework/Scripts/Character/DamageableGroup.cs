using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Damageable Group")]
    public class DamageableGroup : MonoBehaviour, IDamageableGroup
    {
        public HumanBodyBones bone;
        public float damageMultipler = 1;

        private Rigidbody _rigidbody;
        private IDamageable damageable;

        public string uniqueID => throw new System.NotImplementedException();

        private void Awake()
        {
            
        }

        private void Start()
        {
            damageable = GetComponentInParent<IDamageable>();
        }

        public void KinematicOff(bool _kinematicInfo)
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            if (_rigidbody != null)
                _rigidbody.isKinematic = _kinematicInfo;
        }

        public IDamageable GetDamageable()
        {
            return damageable;
        }

        public HumanBodyBones GetBone()
        { 
            return bone;
        }

        public float GetDamageMultipler()
        {
            float multiplier = damageMultipler;

            // 🎯 헤드샷 보정: Head일 때만 적용
            if (bone == HumanBodyBones.Head)
            {
                multiplier *= SkillEffectHandler.Instance.headshotDamageMultiplier;
            }
            // 💢 Berserker Strike 효과 적용 (플레이어 체력 기반 추가 배수)
            if (SkillEffectHandler.Instance.isHeartofBerserkeravailable)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null && player.TryGetComponent(out IDamageable playerDamageable))
                {
                    float maxHp = playerDamageable.playerMaxHealth;
                    float curHp = playerDamageable.health;
                    float missingPercent = Mathf.Clamp01((maxHp - curHp) / maxHp); // 0~1

                    int chunkCount = Mathf.FloorToInt(missingPercent * 10f); // 10% 단위
                    float bonus = SkillEffectHandler.Instance.berserkerDamageMultiplier * chunkCount;

                    multiplier *= 1f + bonus;

                    Debug.Log($"🔥 Heart of Berserker: {chunkCount * 10}% HP 손실 → +{bonus * 100f}% 데미지");
                }
            }

            return multiplier * SkillEffectHandler.Instance.bonusDamegeRate;
        }


    }
}