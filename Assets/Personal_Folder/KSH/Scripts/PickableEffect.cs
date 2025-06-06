using System.Collections.Generic;
using UnityEngine;

public class PickableEffect : MonoBehaviour
{
    public int level = 1;
    public List<GameObject> effects;


    void Start()
    {

            if (effects[level - 1]) Instantiate(effects[level - 1], transform);
        
    }

}
