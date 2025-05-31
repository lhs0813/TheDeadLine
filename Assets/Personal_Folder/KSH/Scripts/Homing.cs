using DunGen;
using UnityEngine;

public class Homing : MonoBehaviour
{
    public GameObject target;
    public float speed = 3;
    public float findRange = 5;
    Collider col;

    void Start()
    {

    }

    void Update()
    {
        if (target)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(col.bounds.center - transform.position), speed * Time.deltaTime);


        }
        else
        {
            target = Info.GetCloseEnemy(gameObject, findRange);
            if(target)
                col = target.GetComponentInChildren<Collider>();
        }
    }
}
