using UnityEngine;
using UnityEngine.Rendering;

public class Set_Post_fx : MonoBehaviour
{
    private Volume _volume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _volume = GetComponent<Volume>();
        _volume.weight = 1;
    }

    // Update is called once per frame

}
