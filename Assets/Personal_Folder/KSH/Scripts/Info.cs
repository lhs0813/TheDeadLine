using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    public int team;


    //최단 적 찾기
    public static GameObject GetCloseEnemy(GameObject from, float radius)
    {
        return GetClosestByList(from, GetEnemybyRange(from, radius));
    }
    static List<GameObject> GetEnemybyRange(GameObject from, float radius)
    {
        //적탐색
        Collider[] cs = Physics.OverlapSphere(from.transform.position, radius, 1 << 6);


        List<GameObject> o = new List<GameObject>();
        for (int i = 0; i < cs.Length; i++)
        {
            if (cs[i] == null)
                continue;

            //var target = cs[i].GetComponentInParent<Hp>();
            //if (target == null)
            //    continue;

            //if (target.isDie)
            //    continue;


            //var targetInfo = target.GetComponentInParent<Info>();
            //if (targetInfo.hideRange > 0 && Vector3.Distance(from.transform.position, target.transform.position) > targetInfo.hideRange)
            //    continue;

            //if (!Info.isDifferTeam(from, target.gameObject))
            //    continue;

            //if (sight == null || !IsVisible(from, target.gameObject, sight.Radious, sight.Angle))
            //    continue;


            //if (o.Contains(target.gameObject) == false)
            //    o.Add(target.gameObject);
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

