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

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            damageable = GetComponentInParent<IDamageable>();
        }

        private void OnEnable()
        {
            _rigidbody.isKinematic = true;
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

            // 🎯 크리티컬 확률 적용 (모든 부위에 적용)
            if (UnityEngine.Random.value <= SkillEffectHandler.Instance.criticalChance)
            {
                multiplier *= SkillEffectHandler.Instance.criticalMultiplier;
                Debug.Log("💥 크리티컬 데미지 발동!");
            }

            // 💢 Desperate Strike 효과 적용 (플레이어 체력 기반 추가 배수)
            if (SkillEffectHandler.Instance.isHeartofBerserkeravailable)
            {
                var player = GameObject.FindWithTag("Player"); // 플레이어 찾기
                if (player != null && player.TryGetComponent(out IDamageable playerDamageable))
                {
                    float currentHp = playerDamageable.health;
                    float extraMultiplier = 1f + (Mathf.Floor((100f - currentHp) / 10f) * 0.1f);
                    multiplier *= extraMultiplier;
                    Debug.Log($"🔥 Desperate Strike 적용됨! 현재 HP: {currentHp}, 배수: x{extraMultiplier}");
                }
            }

            return multiplier;
        }


    }
}