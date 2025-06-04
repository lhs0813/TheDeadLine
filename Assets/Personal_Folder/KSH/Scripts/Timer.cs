using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public bool isRepeat;
    public int num;//0이면 무한반복 / 1이상부터 개수적용
    public float timeRndMax;
    public UnityEngine.Events.UnityEvent OnTime;
    public bool onDestroy;

    int num_count;


    void OnEnable()    
    {        
        if(onDestroy)
            OnTime.AddListener(DestroyThis);

        StartCoroutine(Act());    
    }
    void OnDisable()    {        StopAllCoroutines();    }


    IEnumerator Act()
    {
        if (isRepeat)
        {
            for (; ; )
            {
                yield return new WaitForSeconds(GetTime());
                OnTime.Invoke();

                num_count++;
                if (num_count == num) break;
            }
        }
        else
        {
            yield return new WaitForSeconds(GetTime());
            OnTime.Invoke();
        }
    }


    float GetTime() 
    {
        float f = time;
        if (timeRndMax > 0) f = Random.RandomRange(time, timeRndMax);
        return f;
    }
    public void SeparateParent() { transform.parent = null; }
    public void DestroyThat(GameObject go) { Destroy(go); }
    public void DestroyThis() { Destroy(gameObject); }
    public void Inst(GameObject go) { Instantiate(go,transform.position,transform.rotation); }

}

