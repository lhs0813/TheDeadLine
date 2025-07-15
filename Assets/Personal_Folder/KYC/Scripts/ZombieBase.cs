using Akila.FPSFramework;
using FIMSpace.FProceduralAnimation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public abstract class ZombieBase : MonoBehaviour, IZombie
{
    [SerializeField] private Animator _anim;
    private int _walkIndex;
    private int _runIndex;
    private int _attackIndex;
    private int _deathIndex;
    public Animator Animator => _anim;
    public Transform Player { get; private set; }
    public bool isPreSpawn = false;

    [Header("Zombie Stats")]
    protected virtual float DefaultHealth => 100f;
    protected virtual float DefaultMaxHealth => 100f;


    public virtual bool UseRandomSpeed => true;
    public virtual bool isBombZombie => false;

    

    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float detectionRange = 50f;
    public float attackRange = 2.5f;

    [Header("Zombie Detection Settings")]
    public float visibleDistance = 45f; // 시야 거리
    public float fieldOfViewAngle = 180f; // 시야각
    public float minAttackStartDistance = 20f; // 등 뒤여도 접근하면 추격
    public bool hurt = false;

    [Header("Zombie Collider")]
    public CapsuleCollider collider;
    public RagdollAnimator2 ragdollAnim;


    [Header("Zombie Sounds")]
    public AudioSource audioSource;

    public AudioClip[] patrolClips;
    public AudioClip[] chaseClips;
    public AudioClip[] attackClips;
    public AudioClip[] deathClips;

    [Header("Procedual Zombie")]
    public Material[] materials;
    public GameObject hatSpace;
    public GameObject[] hats;

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


    Actor _playerActor;
    KillFeed killFeed;
    Hitmarker hitmarker;


    protected virtual void Awake()
    {
        health = DefaultHealth;
        maxHealth = DefaultMaxHealth;

        _playerActor = Player_Manager.Instance.GetComponent<Actor>(); // <-- 이현수 수정 액터 0716
        killFeed = UIManager.Instance.KillFeed;
        hitmarker = UIManager.Instance.Hitmarker;

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


    protected virtual void OnEnable() //------0607 김현우 수정 : Pooling 대응, 좀비 초기화는 OnEnable에서 수행.
    {
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
        _runIndex = Random.Range(0, 4);
        _attackIndex = Random.Range(0, 3);
        _deathIndex = Random.Range(0, 2);

        _anim.SetFloat("walkIndex", _walkIndex);
        _anim.SetFloat("runIndex", _runIndex);
        _anim.SetFloat("attackIndex", _attackIndex);
        _anim.SetFloat("deathIndex", _deathIndex);

        #region 일반 절차적 좀비 설정
        if (gameObject.name == "Zombie_Normal")
        {
            Transform target = transform.Find("Zombie_Mesh");
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null && materials.Length > 0)
            {
                Material randomMat = materials[Random.Range(0, materials.Length)];
                renderer.material = randomMat;
            }

            // 기존 모자 제거
            foreach (Transform child in hatSpace.transform)
            {
                Destroy(child.gameObject);
            }

            // 50% 확률로만 모자 생성
            if (Random.value < 0.5f) // 0.5보다 작을 확률은 약 50%
            {
                int randomIndex = Random.Range(0, hats.Length);
                GameObject hat = Instantiate(hats[randomIndex], hatSpace.transform);

                hat.transform.localPosition = Vector3.zero;
                hat.transform.localRotation = Quaternion.identity;
            }
        }
        #endregion

        hurt = false;

        if (identifier != null)
        {
            isPreSpawn = identifier.isPrespawn;
            identifier.wasTrackingPlayer = false;
        }
        else
            isPreSpawn = false;

        if(ragdollAnim != null)
            ragdollAnim.RA2Event_SwitchToStand();

        collider.enabled = true;
        Kinematic_Controll(true);

        health = maxHealth;
        agent.enabled = true;
        SetState(new PatrolState());

        transform.parent.GetComponentInChildren<Damageable>(true).ResetHealth(this);

        if (isBombZombie)
        {
            Explosive _explosive = GetComponent<Explosive>();
            _explosive.health = 100;
            _explosive.exploded = false;
        }
            

        _anim.speed = 1;
        transform.localScale = scaleOrigin;
        var ragdolDummy = transform.parent.GetComponentInChildren<FIMSpace.FProceduralAnimation.RagdollAnimatorDummyReference>(true);
        if (ragdolDummy) ragdolDummy.gameObject.active = true;
    }

    protected virtual void Update()
    {
        if (damageable != null && damageable.health <= 0 && !(currentState is DeadState))
        {
            Die();
            return;
        }
        currentState?.Update();

        if (agent.isOnOffMeshLink)
        {
            OffMeshLinkData data = agent.currentOffMeshLinkData;

            //calculate the final point of the link
            Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

            //Move the agent to the end point
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);

            //when the agent reach the end point you should tell it, and the agent will "exit" the link and work normally after that
            if (agent.transform.position == endPos)
            {
                agent.CompleteOffMeshLink();
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
        else if (currentState is PatrolState)
        {
            Debug.Log("Damaged While patrolling");
            SetState(new ChaseState());
        }
    }

    public void ReleaseSelf()
    {
        EnemyPoolManager.Instance.ReturnToPool(identifier.Type, identifier.gameObject, 0f);
    }

    //0616 김현우 수정 : 추적시작시 패턴 추가.
    public void SetNotBeDespawned() //추적을 시작한 적은 죽을때까지 강제회수되지 않음.
    {
        if (identifier != null)
        {
            identifier.wasTrackingPlayer = true;
        }

    }

    protected virtual void Die()
    {
        SetState(new DeadState());
        //Debug.Log($"{gameObject.name} 사망");

        agent.enabled = false; // NavMeshAgent 비활성화
        collider.enabled = false; // 콜리더 비활성화
        Kinematic_Controll(false);
        if(ragdollAnim != null)
            ragdollAnim.RA2Event_SwitchToFall();

        _playerActor.kills++;


        if (killFeed != null)
        {
            killFeed.Show(_playerActor, "zombie", false);
        }

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
        if (Player_Manager.Instance.playerIsGod) // 이현수 수정 플레이어 무적 관리
            return;

        // 플레이어 오브젝트 확인
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        float distance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(playerObj.transform.position.x, playerObj.transform.position.z));


        if (distance <= attackRange)
        {
            //Debug.Log("공격 타이밍!");

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

                float baseDamage = EnemyConstants.GetZombieDamageByType(identifier.Type, GamePlayManager.instance.currentMapIndex) * SkillEffectHandler.Instance.damageReduction; // 데미지 감소 적용
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
    

}