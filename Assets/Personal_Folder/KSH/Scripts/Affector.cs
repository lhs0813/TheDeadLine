using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Affector : MonoBehaviour
{
    [Header("OnHit")]
    public float damage;
    public float push;
    [Space(30)]


    [Header("Check")]
    public float checkTime = 0.2f; // 전체체크시간 
    public float checkCycle = -1;//체크주기/ 음수면 무한체크
    public LayerMask LayerMask;
    public bool allowRepeat;
    public float hitCount = -1; //음수면 무한재공격
    [Space(30)]


    [Header("Additional")]
    public bool blockWall;
    public bool reflect;
    public GameObject hitEffect;
    public UnityEngine.Events.UnityEvent OnHit;
    public UnityEngine.Events.UnityEvent OnHitCountEnd;
    [Space(30)]


    List<GameObject> hitted = new();//중복방지start
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

        RaycastHit[] hits = Physics.SphereCastAll(before, transform.localScale.x / 2, transform.forward, Vector3.Distance(before, transform.position), LayerMask);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));


        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == null)
                continue;


            if (hits[i].point == Vector3.zero)
                hitPoint = hits[i].transform.position;
            else
                hitPoint = hits[i].point;

            

            hitNormal = hits[i].normal;
            CommonEnter(hits[i].transform.gameObject);
        }


        before = transform.position;
    }

    public void CommonEnter(GameObject go)
    {
        var target = go.GetComponentInParent<Info>();
        if (target == null)
        {
            if (hitted.Contains(go) == true)
                return;




            hitCount--;
            if (hitCount == 0) Destroy(gameObject); // enabled = false;
            if (allowRepeat == false) hitted.Add(go);



            if (reflect)
            {
                transform.position = hitPoint;
                transform.forward = Vector3.Reflect(transform.forward, hitNormal);
            }



            if (hitEffect) Destroy(Instantiate(hitEffect, hitPoint, transform.rotation), 5);
            OnHit.Invoke();
        }
        else
        {

            if (hitted.Contains(target.gameObject) == true)
                return;




            hitCount--;
            if (hitCount == 0) Destroy(gameObject); // enabled = false;
            if (allowRepeat == false) hitted.Add(go);


            //-------------------------------------------------




            if (hitEffect) Destroy(Instantiate(hitEffect, hitPoint, transform.rotation), 5);
            OnHit.Invoke();
        }
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