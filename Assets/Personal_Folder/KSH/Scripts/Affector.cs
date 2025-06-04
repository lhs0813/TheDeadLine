using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Affector : MonoBehaviour
{
    [Header("Affect")]
    public float damage;
    public float push;
    public float slow;
    [Space(30)]


    [Header("Check")]
    public float checkTime = 0.1f; // 전체체크시간 
    public float checkCycle = 99;//체크주기
    //public LayerMask LayerMask;
    public bool allowRepeat;
    public bool hitEnrionment =true;
    public bool hitDamageble=true;
    public bool isRayCast;
    [Space(30)]


    [Header("Hit")]
    public float hitCount = -1; //음수면 무한재공격
    public bool hitMaxDestroy;
    public bool hitFirstIgnore;
    public bool reflect;
    public UnityEngine.Events.UnityEvent OnHit;
    [Space(30)]


    [Header("HitEffect")]
    public GameObject hitEffect;
    public bool efDiretionHitNormal;
    public bool efInChiled;
    public bool efInChiledlocalPositionZero;
    public GameObject hitNext;
    public float efDestroyTime=10;
    [Space(30)]


    List<GameObject> hitted = new();//중복방지start
    bool hitFirstIgnoreCheck;
    GameObject hitGameobject;
    Vector3 hitPoint;
    Vector3 hitNormal;
    Vector3 before;


    void Start()
    {
        before = transform.position;
        if (checkTime > 0) StartCoroutine(CheckTime());
        if (checkCycle > 0) StartCoroutine(CheckCycle());
    } 
    IEnumerator CheckTime()
    {
        yield return new WaitForSeconds(checkTime);
        enabled = false;
    }
    IEnumerator CheckCycle()
    {
        for (; ; )
        {
            CheckTarget();
            yield return new WaitForSeconds(checkCycle);
        }
    }
    private void OnDisable() { StopAllCoroutines(); }



    public void CheckTarget()
    {
        RaycastHit[] hits = null;
        if (isRayCast)
            hits = Physics.RaycastAll(before, transform.forward, Vector3.Distance(before, transform.position));
        else
            hits = Physics.SphereCastAll(before, transform.localScale.x / 2, transform.forward, Vector3.Distance(before, transform.position));

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));


        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == null)
                continue;

            if (hitCount == 0)
                break;

            if (hitFirstIgnore)
            {
                if (hitFirstIgnoreCheck == false)
                {
                    hitFirstIgnoreCheck = true;
                    hitted.Add(hits[i].transform.gameObject);
                    continue;
                }
            }




            if (hits[i].point == Vector3.zero)
                hitPoint = hits[i].transform.position;
            else
                hitPoint = hits[i].point;

            hitNormal = hits[i].normal;



            CommonEnter(hits[i].transform.gameObject);
        }

        before = transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        CommonEnter(collision.gameObject);
    }




    public void CommonEnter(GameObject go)
    {
        //Damagebla 판단 
        var damageTarget = go.GetComponentInParent<Akila.FPSFramework.Damageable>();
        if (damageTarget)
        {
            //팀확인
            if (damageTarget.type == Akila.FPSFramework.HealthType.Player)
                return;

            //중복판별 
            if (hitted.Contains(go) == true)
                return;

            //유닛만 
            if (hitDamageble ==false)
                return;


                                    

            damageTarget.Damage(damage, gameObject, false);

            if (slow > 0)
            {
                var nav = damageTarget.GetComponentInParent<NavMeshAgent>();
                if (nav) nav.speed *= slow;
            }




            CommonAffect(go);
        }

        //지형충돌
        var environment = go.GetComponentInParent<MonoBehaviour>(); 
        if (!environment)
        {
            if (hitted.Contains(go) == true)
                return;

            if (hitEnrionment==false)
                return;




            CommonAffect(go);
        }
    }
    void CommonAffect(GameObject go)
    {
        hitGameobject = go;

        if (allowRepeat == false)
            hitted.Add(go);

        hitCount--;
        if (hitCount == 0)
        {
            if (hitMaxDestroy)
                Destroy(gameObject); // enabled = false;
            else
                enabled = false;
        }




        if (reflect)
        {
            transform.position = hitPoint;
            transform.forward = Vector3.Reflect(transform.forward, hitNormal);
        }


        if(push!=0)
        {
            var rb = go.GetComponentInParent<Rigidbody>();
            if (rb)
                rb.linearVelocity = (go.transform.position - hitPoint).normalized * push;
        }


        if (hitEffect)
        {
            GameObject v = null;
            if (efDiretionHitNormal)
            {
                v = Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(hitNormal));
                v.transform.up = hitNormal;
            }
            else
            {
                v = Instantiate(hitEffect, hitPoint, transform.rotation);
            }


            if (efInChiled)
            {
                v.transform.parent = go.transform;
                if (efInChiledlocalPositionZero) 
                    v.transform.localPosition = Vector3.zero;
            }

            Destroy(v, efDestroyTime);
        }


        if (hitNext)
            Instantiate(hitNext, hitPoint, Quaternion.LookRotation(Vector3.Reflect(transform.forward, hitNormal)));


        OnHit.Invoke();
    }


    public void ThisPosToHitPoint() { transform.position = hitPoint; }
    public void TargetPosToThis()
    {
        var nav = hitGameobject.GetComponentInParent<NavMeshAgent>();
        if(nav) nav.enabled = false;

        hitGameobject.transform.parent = transform; 
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x / 2);
    }
}

/*
 * 
        //    //1회차 or 위치같을때
        //    if (before == transform.position)
        //    {
        //        Collider[] cols = new Collider[30];
        //        Physics.OverlapSphereNonAlloc(transform.position, transform.localScale.x/2, cols, LayerMask);

        //        for (int i = 0; i < cols.Length; i++)
        //        {
        //            if (cols[i] != null)
        //            {
        //                hitPoint = cols[i].ClosestPoint(transform.position);
        //                hitNormal = cols[i].transform.up;
        //                CommonEnter(cols[i].transform.gameObject);
        //            }
        //        }
        //    }
        //    else
            //var cc = target.GetComponent<PlayerControl>();
            //if (cc)
            //    cc.outForce = dir.normalized * push;

            //var rb = target.GetComponent<AISimple>();
            //if (rb)
            //{
            //    rb.outForce = dir.normalized * push;
            //    rb.velocity = Vector3.zero;
            //}

            //var rb = target.GetComponent<AISimple>();
            //if (rb)
            //    rb.outForce = dir.normalized * push;
        //Collider[] cols = new Collider[10];
        //Physics.OverlapSphereNonAlloc(transform.position, transform.localScale.x, cols);
        //스케일을 메인으로  / 콜라이더 반지름0.5  

   // private void OnCollisionEnter(Collision collision)
   // {
   //     hitPoint= collision.contacts[0].point;
   //     CommonEnter(collision.gameObject);
   // }
   // private void OnTriggerEnter(Collider other)
   // {
   //     hitPoint = other.ClosestPoint(transform.position);
   //     CommonEnter(other.gameObject);
   // }

            if (Info.isDiffer(info.owner, go.gameObject) == false
        if (info.owner == go.gameObject)  return;//소유자 방지 

 
 
        var l = go.GetComponentInParent<Info>();  if (l == null) return;
        if (hitted.Contains(l.gameObject) == true) return;
        if (Info.isDiffer(info.owner, go.gameObject) == false) return;


 
 
 */