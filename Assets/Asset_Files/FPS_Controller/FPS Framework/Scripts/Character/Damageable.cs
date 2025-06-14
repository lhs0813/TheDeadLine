using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Damageable")]
    public class Damageable : MonoBehaviour, IDamageable
    {
        

        public HealthType type = HealthType.Other;
        public float health = 100;
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


        private void Awake()
        {


        }

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
            
            if(Actor == null)
            {
                Debug.LogError("Couldn't find Actor on Damageable", gameObject);
                return;
            }

            if(Actor.characterManager == null)
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


        private IEnumerator DelayedLoad()
        {
            yield return new WaitForSeconds(3f); // 3초 대기
            Cursor.lockState = CursorLockMode.None;  // 마우스 잠금 해제
            Cursor.visible = true;                   // 마우스 커서 보이게
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
                EnemyPoolManager.Instance.ReturnToPool(enemyIdentifier.Type, enemyIdentifier.gameObject);
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
            if (type == HealthType.Player && isPlayer)
            {
                float FakeHp = health;
                FakeHp -= amount;
                Player_Manager.PlayerHpChange?.Invoke(FakeHp);
                
            }
            
            health -= amount;


            

            if(_killFeed == null)
            {
                _killFeed = FindAnyObjectByType<KillFeed>();
            }
            if(type == HealthType.NPC)
                _killFeed.DamageShow(amount, critical);

            






            /*KillTag newTag = Instantiate(Tag, tagsHolder);
            newTag.message.color = headshot && newTag.updateImageColors ? headshotColor : newTag.message.color;
            newTag.gameObject.SetActive(true);

            newTag.Show(killer, killed);*/


            this.damageSource = damageSource;
        }

        public bool isActive { get; set; } = true;

        public UnityEvent onDeath => OnDeath;
    }

    public enum HealthType
    {
        Player = 0,
        NPC = 1,
        Other = 2
    }
}