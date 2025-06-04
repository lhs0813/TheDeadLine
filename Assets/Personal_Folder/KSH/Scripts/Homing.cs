using DunGen;
using UnityEngine;

public class Homing : MonoBehaviour
{
    public GameObject target;
    public float rotateSpeed = 3;
    public float findRange = 5;

    Collider col;



    void Update()
    {
        if (target)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(col.bounds.center - transform.position), rotateSpeed * Time.deltaTime);
        }
        else
        {
            target = Info.GetCloseEnemy(gameObject, findRange);
            if(target)
                col = target.GetComponentInChildren<Collider>();
        }
    }
}
