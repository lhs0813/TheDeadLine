using System.Collections;
using UnityEngine;

public class Instance : MonoBehaviour
{
    public GameObject next;
    public int num;
    public float angle;
    public float delay;


    void Start()
    {
      StartCoroutine(Spawn());
    }


    IEnumerator  Spawn()
    {
        Quaternion temp = transform.rotation;
        transform.Rotate(Vector3.up, -num * angle / 2);
        transform.Rotate(Vector3.up, -angle / 2);

        for (int j = 0; j < num; j++)
        {
            transform.Rotate(Vector3.up, angle);
            var o = Instantiate(next, transform.position, transform.rotation);

            yield return new WaitForSeconds(delay);
        }
        transform.rotation = temp;
    }

}
