using UnityEngine;



public class RotateGun : MonoBehaviour
{
    public float val = 200f; // Boss총알 회전 속도 
    public float accel=1;

    float now;
    bool isFireing;

    public void FireStart()
    {
        now = 0;
        isFireing = true;
    }
    public void FireEnd()
    {
        isFireing = false;
    }

    void Update()
    {
        if (isFireing)
            now = Mathf.Lerp(now, val, Time.deltaTime * accel);
        else
            now = Mathf.Lerp(now, 0, Time.deltaTime * accel);

        transform.Rotate(0, 0, 1 * now * Time.deltaTime);
    }

    public void Add(float _val) { val = _val; }
}
