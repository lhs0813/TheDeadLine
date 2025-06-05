using UnityEngine;



public class Rotate : MonoBehaviour
{
    public float val = 200f; // Boss총알 회전 속도 
    GameObject p;

    private void Start()
    {
        p = transform.parent.gameObject;
        transform.parent = null;
    }
    void Update()
    {
        if (p == null)
            Destroy(gameObject);
        else
        {
            transform.position = p.transform.position;
            transform.Rotate(0, 0, 1 * val * Time.deltaTime); //속도를 늘리면 될듯?

        }
    }
}
