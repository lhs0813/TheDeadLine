using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Damageable")]
    public class Damageable : MonoBehaviour, IDamageable
    {
        [Header("Skills Settings")]
        private bool hasUsedInvincibilityInThisStation = false;
        private bool isInvincible = false;
        private float invincibilityEndTime = 0f;

        public HealthType type = HealthType.Other;

        public float health = 100;
        public float playerMaxHealth { get; set; }

        public float destroyDelay;
        [Range(0, 1)] public float damageCameraShake = 0.3f;

        [Space]
        public bool destoryOnDeath;
        public bool destroyRoot;
        public bool Ragdoll_Character;
        public bool ragdolls;
        public GameObject deathEffect;

        [Space]
        public UnityEvent OnDeath;

        public Actor Actor { get; set; }
        public Ragdoll ragdoll { get; set; }
        public GameObject damageSource { get; set; }
        public Vector3 damageDirection { get; set; }
        public float maxHealth { get; set; }
        public IDamageableGroup[] groups { get; set; }
        public bool died;
        public bool deadConfirmed { get; set; }


        //킬 데미지 관련 변수 설정
        KillFeed _killFeed;
        public bool isPlayer = false;

        //------0607 김현우 수정 : Identifier 대응.
        EnemyIdentifier enemyIdentifier; //EnemyPool에서 사용하는 태그 관리 컴포넌트.


        //------0607 김현우 수정 : 죽고 다시 로딩될 시, 체력을 재차 초기화함.
        public void ResetHealth(ZombieBase zombieBase)
        {
            if (zombieBase != null)
            {
                health = zombieBase.health;
                maxHealth = zombieBase.health;
            }
            else
            {
                maxHealth = health;
            }

            died = false;
        }

        private void Start()
        {
            Actor = GetComponent<Actor>();
            ragdoll = GetComponent<Ragdoll>();

            OnDeath.AddListener(Die);

            if (type == HealthType.Player)
            {
                if (Actor && Actor.characterManager != null) DeathCamera.Instance?.Disable();
                playerMaxHealth = health; // 플레이어의 최대 체력을 100 으로 초기화, 만약 기본 체력이 50이면 최대체력도 50일거임; - 이현수;



                groups = GetComponentsInChildren<IDamageableGroup>();

                if (Actor && Actor.characterManager != null)
                {
                    if (UIManager.Instance && UIManager.Instance.HealthDisplay && Actor)
                    {
                        UIManager.Instance.HealthDisplay?.UpdateCard(health, Actor.actorName, false);
                        UIManager.Instance.HealthDisplay.actorNameText.text = Actor.actorName;
                    }
                }
            }

            //--------0607 김현우 수정 : EnemyIdentifier 대응.
            if (type == HealthType.NPC)
            {
                enemyIdentifier = GetComponentInParent<EnemyIdentifier>();
            }

            if (type == HealthType.Other)
            {
                if (ragdoll || Actor) Debug.LogWarning($"{this} has humanoid components and it's type is Other please change type to Humanoid to avoid errors.");
            }

            playerMaxHealth = 100 + SkillEffectHandler.Instance.maxHealthIncreaseAmount;
        }

        public bool allowDamageScreen { get; set; } = true;
        public bool allowRespawn { get; set; } = true;
        float IDamageable.health { get => health; set => health = value; }

        private float previousHealth;

        private void Update()
        {
            if (health != previousHealth)
            {
                if (health > previousHealth)
                {
                }
                else
                {
                    UpdateSystem();
                }

                previousHealth = health;
            }
            if (isInvincible && Time.time >= invincibilityEndTime)
            {
                isInvincible = false;
                Debug.Log("⏱️ 무적 종료됨");
            }

            if (SkillEffectHandler.Instance.maxHealthIncrease && type == HealthType.Player)
            {
                int newMax = 100 + Mathf.RoundToInt(SkillEffectHandler.Instance.maxHealthIncreaseAmount);

                if (playerMaxHealth != newMax)
                {
                    playerMaxHealth = newMax;

                    // 체력 최대치 변경 → 현재 체력이 최대치를 넘지 않도록 정리
                    if (health > playerMaxHealth)
                        health = playerMaxHealth;

                    Player_Manager.PlayerMaxHpChange?.Invoke(playerMaxHealth);
                    Player_Manager.PlayerHpChange?.Invoke(health);
                    Debug.Log($"❤️ 최대 체력 증가됨: {playerMaxHealth}");
                }
            }


        }

        private void UpdateSystem()
        {
            if (!died && health <= 0)
            {
                OnDeath?.Invoke();
            }

            if (type == HealthType.Player && Actor.characterManager != null)
            {
                Actor.characterManager.cameraManager.ShakeCameras(damageCameraShake);
            }

            UpdateUI(1);
        }

        private void UpdateUI(float alpha)
        {
            if (!allowDamageScreen) return;

            if (type != HealthType.Player) return;

            if (Actor == null)
            {
                Debug.LogError("Couldn't find Actor on Damageable", gameObject);
                return;
            }

            if (Actor.characterManager == null)
            {
                Debug.LogError("Couldn't find CharacterManager on Damagable.", gameObject);
                return;
            }

            UIManager uIManager = UIManager.Instance;

            if (uIManager == null)
            {
                Debug.LogError("UIManager is not set. Make sure to have at a UIManager in your scene.", gameObject);
                return;
            }

            if (damageSource != null)
                UIManager.Instance?.DamageIndicator?.Show(damageSource.transform.position, alpha);

            UIManager.Instance?.HealthDisplay?.UpdateCard(health, Actor.actorName, true);
        }


        private IEnumerator DelayedLoad() // 사망시 메인메뉴 씬으로 돌아가는 시스템 - 이현수
        {
            yield return new WaitForSeconds(3f); // 3초 대기
            UnityEngine. Cursor.lockState = CursorLockMode.None;  // 마우스 잠금 해제
            UnityEngine.Cursor.visible = true;                   // 마우스 커서 보이게
            SceneManager.LoadScene("Main Menu");
        }
        private void Die()
        {
            //---------0607 김현우 수정 : EnemyIdentifier 대응.
            if (!isActive) return;


            if (type == HealthType.Player)
            {
                if (Actor.respawnable) Actor.deaths++;
                if (damageSource) DeathCamera.Instance?.Enable(gameObject, damageSource);

                StartCoroutine(DelayedLoad());
            }

            if (ragdoll) ragdoll.Enable(damageDirection);
            if (deathEffect) Instantiate(deathEffect, transform.position, transform.rotation);


            // 풀로 반환: EnemyIdentifier에서 타입을 꺼내서 ReturnToPool 호출
            if (enemyIdentifier != null)
            {
                Debug.Log($"{enemyIdentifier} 반환.");
                EnemyPoolManager.Instance.ReturnToPool(enemyIdentifier.Type, enemyIdentifier.gameObject, 5f);
            }
            else
            {
                /*// 풀용 오브젝트가 아니면 원래대로 Destroy
                if (destoryOnDeath && !destroyRoot)
                    Destroy(gameObject, destroyDelay);
                else if (destoryOnDeath && destroyRoot)
                    Destroy(transform.parent.gameObject, destroyDelay);*/
            }

            died = true;
        }

        public UnityEvent onRespawn;

        private void Respwan()
        {
            if (type == HealthType.Other || !Actor) return;

            onRespawn?.Invoke();

            if (Actor.respawnable)
            {
                Actor.Respwan(SpawnManager.Instance.respawnDelay);
            }
        }

        public void Damage(float amount, GameObject damageSource, bool critical)
        {
            //적이 받는 데미지 전체 조정 
            if (type != HealthType.Player)
                amount *= Affector.damageMulti;

            //방패판정 
            if (type == HealthType.Player)
            {
                if (isBlocked(damageSource))
                    return;
            }


            if (type == HealthType.Player && isPlayer)
            {
                float predictedHp = health - amount;

                // 스킬이 적용 중이고, 체력이 20 이하로 떨어지며, 아직 해당 역에서 한 번도 발동되지 않은 경우
                if (SkillEffectHandler.Instance.isInvinciblePerStation &&
                    predictedHp <= 19 && !hasUsedInvincibilityInThisStation)
                {
                    Debug.Log("🛡️ 무적 발동됨");
                    isInvincible = true;
                    invincibilityEndTime = Time.time + 3f;
                    hasUsedInvincibilityInThisStation = true;
                    return; // 데미지 무시
                }

                // 무적 상태면 데미지 무시
                if (isInvincible)
                {
                    Debug.Log("💥 데a미지 무시됨 (무적 중)");
                    return;
                }

                Player_Manager.PlayerHpChange?.Invoke(predictedHp); // 가상 체력 UI 처리
            }

            health -= amount;




            if (_killFeed == null)
            {
                _killFeed = FindAnyObjectByType<KillFeed>();
            }
            if (type == HealthType.NPC)
                _killFeed.DamageShow(amount, critical);



            /*KillTag newTag = Instantiate(Tag, tagsHolder);
            newTag.message.color = headshot && newTag.updateImageColors ? headshotColor : newTag.message.color;
            newTag.gameObject.SetActive(true);

            newTag.Show(killer, killed);*/


            this.damageSource = damageSource;
        }
        public void ResetInvincibilityFlagPerStation()
        {
            hasUsedInvincibilityInThisStation = false;
        }

        public bool isActive { get; set; } = true;

        public UnityEvent onDeath => OnDeath;




        bool isBlocked(GameObject _from)
        {
            var from = _from.transform.position ; from.y = 0;
            var to = transform.position ; to.y = 0;
            var dir = to - from; dir.Normalize();
            var fwd = GetComponentInChildren<Pinger>().transform.forward; fwd.y = 0;


            if (Vector3.Angle(fwd, dir) > 120)
            {
                var block = transform.GetComponentInChildren<Blockable>();
                if (block)
                {
                    block.Block();
                    return true;
                }
            }
            return false;

            //System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));


            //for (var i = 0; i < hits.Length; i++)
            //{
            //    var block = hits[i].transform.GetComponent<Blockable>();
            //    if (block)
            //    {
            //        block.Block();
            //       // return true;
            //    }


            //   var dam = hits[i].transform.GetComponentInParent<Damageable>();
            //    if(dam)
            //    {
            //        //return false;
            //    }
            //}
            //return false;
        }

    }


    public enum HealthType
    {
        Player = 0,
        NPC = 1,
        Other = 2
    }

}