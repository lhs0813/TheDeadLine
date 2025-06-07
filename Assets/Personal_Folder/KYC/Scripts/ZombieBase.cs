using Akila.FPSFramework;
using UnityEngine;

public abstract class ZombieBase : MonoBehaviour, IZombie
{
    private Animator _anim;
    private int _walkIndex;
    private int _runIndex;
    private int _attackIndex;
    public Animator Animator => _anim;

    [Header("Zombie Stats")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float detectionRange = 50f;
    public float attackRange = 2.5f;

    protected IZombieState currentState;
    protected UnityEngine.AI.NavMeshAgent agent;
    public UnityEngine.AI.NavMeshAgent Agent => agent;
    //------0607 김현우 수정 : Damagable 컴포넌트 받아오기.ㄴ
    Damageable damageable;

    protected virtual void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent 컴포넌트가 없음!");
        }
        else
        {
            agent.speed = moveSpeed;
        }
        _anim = GetComponent<Animator>();

        _walkIndex = Random.Range(0, 3);
        _runIndex = Random.Range(0, 3);
        _attackIndex = Random.Range(0, 3);

        _anim.SetFloat("walkIndex", _walkIndex);
        _anim.SetFloat("runIndex", _runIndex);
        _anim.SetFloat("attackIndex", _attackIndex);

        damageable = GetComponentInChildren<Damageable>();
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

    private void InitializeZombieState()
    {
        health = maxHealth;
        SetState(new PatrolState());

        transform.parent.GetComponentInChildren<Damageable>().ResetHealth(this);
    }

    protected virtual void Update()
    {
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

    protected virtual void Die()
    {
        SetState(new DeadState());
        Debug.Log($"{gameObject.name} 사망");
        Destroy(gameObject, 2f);
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
        // 플레이어가 null인지 체크
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        float distance = Vector3.Distance(transform.position, playerObj.transform.position);
        if (distance <= attackRange)
        {
            Debug.Log("공격 타이밍!");
        }
    }
}