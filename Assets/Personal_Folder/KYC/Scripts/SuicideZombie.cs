using UnityEngine;

public class SuicideZombie : ZombieBase
{
    public float explosionRadius = 4f;
    public float explosionDamage = 80f;

    protected override void Start()
    {
        health = 60f;
        moveSpeed = 2.5f;
        base.Start();
    }

    protected override void Die()
    {
        // 폭발 처리
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            IZombie zombie = hit.GetComponent<IZombie>();
            if (zombie != null && zombie != this)
            {
                zombie.TakeDamage(explosionDamage / 2f); // 주변 좀비에게도 반 피해
            }

            if (hit.CompareTag("Player"))
            {
                // 플레이어에게 폭발 데미지
            }
        }

        Debug.Log("자폭 좀비 폭발!");
        base.Die();
    }
}
