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

        private IDamageable damageable;

        public string uniqueID => throw new System.NotImplementedException();

        private void Start()
        {
            damageable = GetComponentInParent<IDamageable>();
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
            if (Random.value <= SkillEffectHandler.Instance.criticalChance)
            {
                multiplier *= SkillEffectHandler.Instance.criticalMultiplier;
                Debug.Log("💥 크리티컬 데미지 발동!");
            }

            return multiplier;
        }

    }
}