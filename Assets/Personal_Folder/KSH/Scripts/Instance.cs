using UnityEngine;

public class Instance : MonoBehaviour
{
    public GameObject next;
    public int num;
    public float angle;


    void Start()
    {
        Spawn();
    }


   public   void Spawn()
    {
        Quaternion temp = transform.rotation;
        transform.Rotate(transform.forward, -num * angle / 2);
        transform.Rotate(transform.forward, -angle / 2);
        for (int j = 0; j < num; j++)
        {
            transform.Rotate(transform.forward, angle);
            var o = Instantiate(next, transform.position, transform.rotation);
        }
        transform.rotation = temp;
    }

}
