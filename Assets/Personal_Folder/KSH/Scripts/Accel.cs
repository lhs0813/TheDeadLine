using UnityEngine;
using System.Collections;


public class Accel : MonoBehaviour
{
    public GameObject target;

    public float accel = 15;
    public float velocityMax = 30;
    public float rotateSpeed = 1;
    public float distMax = 20;
    public float break2 = 1;
    public float findRange = 30;

    public Vector3 all;
    public Vector3 to;
    Coroutine c_accel;
    Coroutine c_break;

    bool isClose;
    Info info;


    void Awake()
    {
        info = GetComponent<Info>();
    }
    void OnEnable()
    {
        c_accel = StartCoroutine(Accel2());
        isClose = true;
    }



    void Update()
    {
        if (target)
        {
            to = target.transform.position;
        }
        else
        {
             target = Info.GetCloseEnemy(gameObject, findRange);
        }



            transform.position += all * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, all, Time.deltaTime * rotateSpeed);



        //안 -> 밖 
        if (isClose == true)
        {
            //거리 멀어지면 기존감속 새로가속
            if (Vector3.Distance(to, transform.position) >= distMax)
            {
                StartCoroutine(Break(new Vector3(all.x, all.y, all.z)));
                all = Vector3.zero;

                if (c_accel != null) { StopCoroutine(c_accel); c_accel = null; }
                c_accel = StartCoroutine(Accel2());

                isClose = false;
            }
        }
        //밖 -> 안
        else
        {

            if (Vector3.Distance(to, transform.position) < distMax)
                isClose = true;
        }
    }
    IEnumerator Accel2()
    {
        for (; ; )
        {
            Vector3 dir = to - transform.position; dir.Normalize();
            all += dir * accel * Time.deltaTime;



            //최대제한
            if (all.magnitude > velocityMax)
                all = all.normalized * velocityMax;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    IEnumerator Break(Vector3 velocity)
    {
        for (; ; )
        {
            velocity = Vector3.Lerp(velocity, default, break2 * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;

            if (velocity.magnitude < 0.1f)
                break;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        all = Vector3.zero;
    }
}






/*
 * 
 * 
 * using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


public class Homing : MonoBehaviour
{
    public bool tagetMode;
    public GameObject target;
    public Vector3 to;

    public float accel = 10;
    public float velocityMax=10;
    public float rotate = 0.2f;
    public float distMax = 10;
    public float break2=3;

    public Vector3 all;
    public Coroutine c_accel;
    public Coroutine c_break;

    bool isClose;
    Info info;

    void Start()
    {
        info=GetComponent<Info>();

        if(tagetMode && info.target)target = info.target;


        c_accel = StartCoroutine(Accel());
        isClose = true;
    }


    void Update()
    {
        if (tagetMode)
        {
            target = Info.GetCloseEnemy(gameObject);
            to = target.transform.position;
        }
        else
        {
            to = info.to;
        }



        transform.position += all * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, all, Time.deltaTime * rotate);



        //안 -> 밖 
        if (isClose == true)
        {
            //거리 멀어지면 기존감속 새로가속
            if (Vector3.Distance(to, transform.position) >= distMax)
            {
                StartCoroutine(Break(new Vector3(all.x, all.y, all.z)));
                all = Vector3.zero;

                if (c_accel != null) { StopCoroutine(c_accel); c_accel = null; }
                c_accel = StartCoroutine(Accel());

                isClose = false;
            }
        }
        //밖 -> 안
        else
        {

            if (Vector3.Distance(to, transform.position) < distMax)
                isClose = true;
        }
    }
    IEnumerator Accel()
    {
        for (; ; )
        {
            Vector3 dir = to - transform.position; dir.Normalize();
            all += dir * accel * Time.deltaTime;



            //최대제한
            if (all.magnitude > velocityMax)
                all = all.normalized * velocityMax;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    IEnumerator Break(Vector3 velocity)
    {
        for (; ; )
        {
            velocity = Vector3.Lerp(velocity, default, break2 * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;

            if (velocity.magnitude < 0.1f)
                break;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}












    IEnumerator Accel()
    {
        var delta = Time.deltaTime;
        float timer = 0;
        float timerMax = 3;

        for (; ; )
        {
            timer += delta;
            velocityNow += accel * delta;


            //최대제한
            if (velocityNow > velocityMax)
                velocityNow = velocityMax;


            if (timer > timerMax)
            {
                c_break = StartCoroutine(Break());
                StopCoroutine(c_accel);
                break;
            }


            yield return new WaitForSeconds(delta);
        }
    }
    IEnumerator Break()
    {
        var delta = Time.deltaTime;
        for (; ; )
        {
            velocityNow = Mathf.Lerp(velocityNow, default, break2 * delta);


            Vector3 dir = (to - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotate);


            if(Vector3.Angle(transform.forward, dir) <1)
            {
                c_accel = StartCoroutine(Accel());
                StopCoroutine(c_break);
                break;
            }

            yield return new WaitForSeconds(delta);
        }
    }



/*
   if (c_accel != null)
                //StopCoroutine(c_accel); c_accel = null;
                //StartCoroutine(Break(new Vector3(all.x, all.y, all.z)));
                //all = Vector3.zero;
                //
                //c_accel = StartCoroutine(Accel());
         all += (p.transform.position - transform.position).normalized * speed 
            * Time.deltaTime;
//  all = all.normalized * max;
// var v = Vector3.Distance(p.transform.position, transform.position);

//transform.rotation = Quaternion.Slerp(transform.rotation, 
//    Quaternion.LookRotation( p.transform.position - transform.position), speed * Time.deltaTime);
 */