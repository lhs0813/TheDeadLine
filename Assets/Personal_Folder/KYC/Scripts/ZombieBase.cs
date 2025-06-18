using Akila.FPSFramework;
using FIMSpace.FProceduralAnimation;
using UnityEngine;

public abstract class ZombieBase : MonoBehaviour, IZombie
{
    private Animator _anim;
    private int _walkIndex;
    private int _runIndex;
    private int _attackIndex;
    private int _deathIndex;
    public Animator Animator => _anim;
    public Transform Player { get; private set; }
    public bool isPreSpawn = false;

    [Header("Zombie Stats")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float detectionRange = 50f;
    public float attackRange = 2.5f;

    [Header("Zombie Detection Settings")]
    public float visibleDistance = 15f; // 시야 거리
    public float fieldOfViewAngle = 120f; // 시야각
    public float minAttackStartDistance = 3f; // 등 뒤여도 접근하면 추격

    [Header("Zombie Collider")]
    public CapsuleCollider collider;
    public RagdollAnimator2 ragdollAnim;


    [Header("Zombie Sounds")]
    public AudioSource audioSource;

    public AudioClip[] patrolClips;
    public AudioClip[] chaseClips;
    public AudioClip[] attackClips;
    public AudioClip[] deathClips;


    protected IZombieState currentState;
    protected UnityEngine.AI.NavMeshAgent agent;
    public UnityEngine.AI.NavMeshAgent Agent => agent;
    //------0607 김현우 수정 : Damagable 컴포넌트 받아오기.ㄴ
    Damageable damageable;
    Vector3 scaleOrigin;

    //------0616 김현우 수정 : identifier에 어그로 변수 추가.
    EnemyIdentifier identifier;

    //0612 이현수 수정 자식 데미저블 그룹 가져오기
    DamageableGroup[] damageableGroups;

    protected virtual void Awake()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            Player = playerObj.transform;
        //0612 이현수 수정 자식 데미저블 그룹 가져오기
        damageableGroups = GetComponentsInChildren<DamageableGroup>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent 컴포넌트가 없음!");
        }
        else
        {
            agent.speed = moveSpeed;
        }

        damageable = GetComponentInChildren<Damageable>();
        identifier = GetComponentInParent<EnemyIdentifier>();
        scaleOrigin = transform.localScale;
    }

    public void SetState(IZombieState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    protected virtual void Start()
    {
        maxHealth = health;
    }


    void OnEnable() //------0607 김현우 수정 : Pooling 대응, 좀비 초기화는 OnEnable에서 수행.
    {
        Debug.Log("OnEnable");
        InitializeZombieState();
    }

    void Kinematic_Controll(bool _kinematicInfo)
    {
        foreach (var group in damageableGroups)
        {
            group.KinematicOff(_kinematicInfo); // 혹은 다른 메서드
        }
    }

    private void InitializeZombieState() // 0609 이현수 수정, 콜리더 활성화 및 래그돌 Standing
    {
        _anim = GetComponent<Animator>();

        _walkIndex = Random.Range(0, 4);
        _runIndex = Random.Range(0, 3);
        _attackIndex = Random.Range(0, 3);
        _deathIndex = Random.Range(0, 2);

        _anim.SetFloat("walkIndex", _walkIndex);
        _anim.SetFloat("runIndex", _runIndex);
        _anim.SetFloat("attackIndex", _attackIndex);
        _anim.SetFloat("deathIndex", _deathIndex);


        if (identifier != null)
        {
            isPreSpawn = identifier.isPrespawn;
            identifier.wasTrackingPlayer = false;
        }
        else
            isPreSpawn = false;

        ragdollAnim.RA2Event_SwitchToStand();
        collider.enabled = true;
        Kinematic_Controll(true);

        health = maxHealth;
        agent.enabled = true;
        SetState(new PatrolState());

        transform.parent.GetComponentInChildren<Damageable>(true).ResetHealth(this);
        _anim.speed = 1;
        transform.localScale = scaleOrigin;
        var ragdolDummy = transform.parent.GetComponentInChildren <FIMSpace.FProceduralAnimation.RagdollAnimatorDummyReference >(true);
        if (ragdolDummy)ragdolDummy.gameObject.active = true;
    }

    protected virtual void Update()
    {
        if (damageable != null && damageable.health <= 0 && !(currentState is DeadState))
        {
            Die();
            return;
        }
        currentState?.Update();
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    //0616 김현우 수정 : 추적시작시 패턴 추가.
    public void SetNotBeDespawned() //추적을 시작한 적은 죽을때까지 강제회수되지 않음.
    {
        identifier.wasTrackingPlayer = true;
    }

    protected virtual void Die()
    {
        SetState(new DeadState());
        Debug.Log($"{gameObject.name} 사망");

        agent.enabled = false; // NavMeshAgent 비활성화
        collider.enabled = false; // 콜리더 비활성화
        Kinematic_Controll(false);
        ragdollAnim.RA2Event_SwitchToFall();
    }

    public virtual void MoveTowards(Vector3 target)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target);
        }
    }

    public void StopMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public void ResumeMovement()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    public virtual bool IsPlayerInRange(Transform player)
    {
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public void AttackEnd()
    {
        if (currentState is AttackState)
        {
            ((AttackState)currentState).OnAttackEnd();
        }
    }
    public void DamageTiming()
    {
        // 플레이어 오브젝트 확인
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;
        if (isBlocked()) { Debug.Log("공격 막힘!"); return; }


        float distance = Vector3.Distance(transform.position, playerObj.transform.position);
        if (distance <= attackRange)
        {
            Debug.Log("공격 타이밍!");

            // Damageable 컴포넌트 가져오기
            var damageable = playerObj.GetComponent<Damageable>();
            if (damageable != null)
            {
                float evasionChance = SkillEffectHandler.Instance.evasionChance;
                float roll = Random.value; // 0.0f ~ 1.0f 사이 랜덤값

                if (roll < evasionChance)
                {
                    Debug.Log("⚡ 회피 성공! 데미지를 받지 않음");
                    return; // 회피 성공 시 데미지 무시
                }

                float baseDamage = 10f * SkillEffectHandler.Instance.damageReduction; // 데미지 감소 적용
                damageable.Damage(baseDamage, this.gameObject, false);
            }
            else
            {
                Debug.LogWarning("플레이어에 Damageable 컴포넌트가 없음");
            }
        }
    }
    public bool CanSeePlayer(Transform player)
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle <= fieldOfViewAngle * 0.5f)
        {
            // 시야각 안에 있음 → Raycast로 장애물 검사
            Ray ray = new Ray(transform.position + Vector3.up * 1.5f, directionToPlayer);
            if (Physics.Raycast(ray, out RaycastHit hit, visibleDistance))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool isBlocked() 
    {
        var from = transform.position + Vector3.up/2;
        var to = FindAnyObjectByType<FirstPersonController>().transform.position + Vector3.up / 2;
        var hits = Physics.RaycastAll(from, to - from, attackRange);

        for (var i = 0; i < hits.Length; i++) 
        {
            var block = hits[i].transform.GetComponent<Blockable>();
            if (block)
            {
                block.Block();
                return true;
            }
        }
        return false;
    }
}