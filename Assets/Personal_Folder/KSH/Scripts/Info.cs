using Akila.FPSFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Info : MonoBehaviour
{

    [Header("Visual")]
    public List<GameObject> visual;
    [Space(30)]


    [Header("Acting")]
    public GameObject next;
    public float endTime = -1;
    public bool forwardWolrdUp;
    public bool forwardXZ;
    public float force;
    public float rotateRnd;
    [Space(30)]



    [Header("Info")]
    public HealthType type;
    public GameObject target;
    public GameObject before;




    void Start()
    {
        if (visual.Count > 0)
            Instantiate(visual[UnityEngine.Random.Range(0, visual.Count)], transform);



        if (endTime>=0)
            Destroy(gameObject, endTime);


        if (rotateRnd > 0)
        {
            transform.Rotate(transform.right, Random.Range(-rotateRnd, rotateRnd)); //랜덤 Y축 회전
            transform.Rotate(transform.up, Random.Range(-rotateRnd, rotateRnd)); //랜덤 Y축 회전
        }


        if (forwardWolrdUp)
            transform.forward = Vector3.up;

        if (forwardXZ)
        { 
            var v = transform.forward;
            v.y = 0; v.Normalize();
            transform.forward = v; 
        }

        if (force > 0)
            GetComponent<Rigidbody>().AddForce(transform.forward * force);

        //-------------------------------------------------------------------------------------------------------------------------------



    }


    public void Next()
    {
        Instantiate(next, transform.position, transform.rotation);
    }






    //최단 적 찾기
    public static GameObject GetCloseEnemy(GameObject from, float radius)
    {
        return GetClosestByList(from, GetEnemybyRange(from, radius));
    }
    static List<GameObject> GetEnemybyRange(GameObject from, float radius)
    {
        //적탐색
        Collider[] cs = Physics.OverlapSphere(from.transform.position, radius);


        List<GameObject> o = new();
        for (int i = 0; i < cs.Length; i++)
        {
            if (cs[i] == null)
                continue;

            var target = cs[i].GetComponentInParent<Damageable>();
            if (target == null)
                continue;

            if (target.deadConfirmed)
                continue;

            if (target.type == HealthType.Player)
                continue;



            if (o.Contains(target.gameObject) == false)
                o.Add(target.gameObject);           
        }

        return o;
    }
    static GameObject GetClosestByList(GameObject fr, List<GameObject> list)
    {
        List<GameObject> gos = list;
        float min = Mathf.Infinity;
        GameObject close = null;
        Vector3 now = fr.transform.position;

        for (int i = 0; i < gos.Count; i++)    //Enemy
        {
            float dist = (gos[i].transform.position - now).sqrMagnitude;
            if (dist < min)//더 가까운 애 발견
            {
                min = dist;
                close = gos[i];
            }
        }
        return close;
    }
}




/*
            //var targetInfo = target.GetComponentInParent<Info>();
            //if (targetInfo.hideRange > 0 && Vector3.Distance(from.transform.position, target.transform.position) > targetInfo.hideRange)
            //    continue;

            //if (sight == null || !IsVisible(from, target.gameObject, sight.Radious, sight.Angle))
            //    continue;
 */