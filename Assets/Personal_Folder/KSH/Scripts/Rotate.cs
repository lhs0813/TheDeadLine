using UnityEngine;



public class Rotate : MonoBehaviour
{
    public float val = 200f; // Boss총알 회전 속도 


    void Update()
    {
        transform.Rotate(0, 0, 1 * val * Time.deltaTime);
    }

    public void Add(float _val) { val = _val; }
}
