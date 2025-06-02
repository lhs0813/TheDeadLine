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
    public float moveSpeed = 2f;
    public float detectionRange = 10f;

    protected IZombieState currentState;
    protected UnityEngine.AI.NavMeshAgent agent;
    public UnityEngine.AI.NavMeshAgent Agent => agent;
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

        // 좀비마다 애니메이션 인덱스 랜덤 설정
        _walkIndex = Random.Range(0, 3);   // Walk 0~2
        _runIndex = Random.Range(0, 4);    // Run 0~3
        _attackIndex = Random.Range(0, 3); // Attack 0~


        _anim.SetInteger("walkIndex", _walkIndex);
        _anim.SetInteger("runIndex", _runIndex);
        _anim.SetInteger("attackIndex", _attackIndex);

        _anim.SetTrigger("TriggerWalk");
    }

    public void SetState(IZombieState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    protected virtual void Start()
    {
        SetState(new PatrolState());
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


}
