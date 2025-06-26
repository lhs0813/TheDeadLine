using System.Collections;
using UnityEngine;

public class Catinbox : MonoBehaviour
{
    public GameObject go;
    public float speed;

    public GameObject out1;
    public GameObject in1;
    Akila.FPSFramework.Firearm firearm;


    void Start()
    {
        firearm = GetComponentInParent<Akila.FPSFramework.Firearm>();


        firearm.onFire += EvtFire;
    }


    void EvtFire(Vector3 a, Quaternion b, Vector3 z) 
    {
        StartCoroutine(Loop());



    }
    IEnumerator Loop()
    {
        go.transform.position = Vector3.Lerp(go.transform.position,out1.transform.position, speed*Time.deltaTime);
        yield return new WaitForSeconds(1);

        
    }
}
