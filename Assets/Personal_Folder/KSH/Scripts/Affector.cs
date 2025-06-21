using Akila.FPSFramework;
using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class Affector : MonoBehaviour
{
    [Header("Affect")]
    public float damage;
    public float push;
    public float motionMultifly=-1; //0 ~ 1 ~ 99999
    public float navMultifly=-1; //0 ~ 1 ~ 99999
    public Vector3 sizeMultifly; //0 ~ 1 ~ 99999
    public bool disableCollider;
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

    public static float damageMulti=1f;

    public List<GameObject> hitted = new();//중복방지start
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
            if(allowRepeat)
                hitted.Clear();

            CheckTarget();
            yield return new WaitForSeconds(checkCycle);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();


        for (int i = 0; i < inTransform.Count; ++i)
        { 
            inTransform[i].transform.parent = null;
        }
    }



    public void CheckTarget()
    {
        RaycastHit[] hits = null;
        if (isRayCast)hits = Physics.RaycastAll(before, transform.forward, Vector3.Distance(before, transform.position));
        else hits = Physics.SphereCastAll(before, transform.localScale.x / 2, transform.forward, Vector3.Distance(before, transform.position));

        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));


        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == null)
                continue;

            if (hits[i].collider.isTrigger)
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
        var damageTarget = go.GetComponentInParent<Damageable>();
        if (damageTarget)
        {
            var target = damageTarget.gameObject;
            var ragdol = damageTarget.GetComponent<RagdollAnimatorDummyReference>();
            if (ragdol)
                target = ragdol.ParentComponent.transform.parent.gameObject;



            if (damageTarget.died)
                return;

            //팀확인
            if (damageTarget.type == HealthType.Player)
                return;

            //중복판별 
            if (hitted.Contains(target.gameObject) == true)
                return;

            //유닛만 
            if (hitDamageble == false)
                return;



            if (damage != 0)
            {
                var value = damage ;
                var critical=false;

                var damageableGroup = go.GetComponent<DamageableGroup>();
                if (damageableGroup)
                    value*=damageableGroup.GetDamageMultipler();


                damageTarget.Damage(value, gameObject, critical);
            }

                UIManager uiManager = UIManager.Instance;


            bool shouldHighlight = damageTarget.health <= 0; // 0616 이현수 0.3 곱 왜한지 몰라서 수정 체력 0이하일 때 킬 효과
            
            if (uiManager != null)
            {
                Hitmarker hitmarker = uiManager.Hitmarker;
                hitmarker?.Show(shouldHighlight);
            }
            


            if(push!=0)
            {
                //var rb = damageTarget.GetComponentInChildren<Rigidbody>();
                //rb.isKinematic = false;
                //rb.linearVelocity = transform.forward* push;
            }



            if (motionMultifly >= 0)
            {
                var ani = target.GetComponentInChildren<Animator>();
                if (ani)
                {
                    ani.speed *= motionMultifly;
                }
            }
            if (navMultifly >= 0)
            {
                var nav = target.GetComponentInChildren<NavMeshAgent>(); 
                if (nav)
                {
                    nav.speed *= navMultifly;
                    nav.angularSpeed *= navMultifly;
                }
            }
            if (sizeMultifly !=Vector3.zero)
            {
                var ani = target.GetComponent<RagdollAnimator2>();
                if (ani)
                {
                    damageTarget.gameObject.active = false;
                    ani.enabled = false;
                }

                Vector3 temp = target.transform.localScale;
                temp.x *= sizeMultifly.x;
                temp.y *= sizeMultifly.y;
                temp.z *= sizeMultifly.z;
                target.transform.localScale = temp;
            }
            if (disableCollider)
            {
               //var v= target.GetComponentsInChildren<Collider>();
               // for(int i = 0; i < v.Length; i++) 
               //     v[i].enabled = false; 
            }



            CommonAffect(target.gameObject);
        }

        //지형충돌
        var environment = go.GetComponent<MonoBehaviour>(); 
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
                rb.linearVelocity = (go.transform.position - transform.position).normalized * push;
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
        //var nav = hitGameobject.GetComponentInParent<NavMeshAgent>();
        //if(nav) nav.enabled = false;

        hitGameobject.transform.parent = transform;
        inTransform.Add (hitGameobject);
    }
   List< GameObject >inTransform = new();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x / 2);
    }
}

/*
 * 
 *  //Damagebla 판단 
        var damageTarget = go.GetComponentInParent<Akila.FPSFramework.Damageable>();
        if (damageTarget)
        {
            var target = damageTarget.gameObject;

            var ragdol = damageTarget.GetComponent<RagdollAnimatorDummyReference>();
            if (ragdol)            
                target = ragdol.ParentComponent.gameObject;
            



            if (damageTarget.died)
                return;

            //팀확인
            if (damageTarget.type == Akila.FPSFramework.HealthType.Player)
                return;

            //중복판별 
            if (hitted.Contains(target.gameObject) == true)
                return;

            //유닛만 
            if (hitDamageble == false)
                return;

                       
                                               


            damageTarget.Damage(damage, gameObject, false);
            if (motionMultifly >= 0)
            {
                var ani = target.GetComponentInParent<Animator>();
                if (ani)
                {
                    ani.speed *= motionMultifly;
                }
            }
            if (navMultifly >= 0)
            {
                var nav = target.GetComponentInParent<NavMeshAgent>(); 
                if (nav)
                {
                    nav.speed *= navMultifly;
                    nav.angularSpeed *= navMultifly;
                }
            }
            if (sizeMultifly !=Vector3.zero)
            {
                Vector3 temp = damageTarget.transform.localScale;
                temp.x *= sizeMultifly.x;
                temp.y *= sizeMultifly.y;
                temp.z *= sizeMultifly.z;
                damageTarget.transform.localScale = temp;

            }
            if (disableCollider)
            {
               var v= target.GetComponentsInChildren<Collider>();
                for(int i = 0; i < v.Length; i++) 
                    v[i].enabled = false; 
            }





            CommonAffect(target.gameObject);
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