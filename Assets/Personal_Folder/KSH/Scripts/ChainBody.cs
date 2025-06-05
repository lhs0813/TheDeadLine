using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBody : MonoBehaviour
{
    public GameObject follow { get; set; }
    public float distance { get; set; }

    void Update()
    {
        if (follow == null)
        {
            Destroy(gameObject);
            return;
        }


        Vector3 dir = follow.transform.position - transform.position;
        if (Vector3.Distance(follow.transform.position, transform.position) > distance)
            transform.position = follow.transform.position - dir.normalized * distance;

        transform.rotation = Quaternion.LookRotation(dir);
    }
}
