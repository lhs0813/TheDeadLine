using Akila.FPSFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PickableEffect : MonoBehaviour
{
    public List<GameObject> effects;
    public List<Material> material;
    public GameObject trail;

    void Start()
    {
        var level = GetComponent<Pickable>().item.GetComponent<Firearm>().gradeNum;
        if (effects[level - 1])
            Instantiate(effects[level - 1], transform);



        var renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            Material[] newMaterials = new Material[materials.Length + 1];
            materials.CopyTo(newMaterials, 0); 
            newMaterials[materials.Length] = material[level-1];
            renderers[i].materials = newMaterials;
        }
    }
}
