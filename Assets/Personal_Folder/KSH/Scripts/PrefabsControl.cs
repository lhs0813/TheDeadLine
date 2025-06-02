using Akila.FPSFramework;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsControl : MonoBehaviour
{
    public List<GameObject> list;

        void Start()
    {
        WeaponSetting();
    }




    [Header("Acting")]
    public FirearmPreset data;

    void WeaponSetting()
    {
        Firearm firearm = null;
        Pickable pickable = null;
        Projectile projectile = null;
        for (int i = 0; i < list.Count; i++)
        {
           var v = list[i].GetComponent<Firearm>();
            if(v) 
                firearm = v;

            var v1= list[i].GetComponent<Pickable>();
            if(v1)
                 pickable = v1;

            var v2 = list[i].GetComponent<Projectile>();
            if (v2)
                projectile = v2;
        }

        data.replacement = pickable;
        data.projectile = projectile;

        firearm.Name = firearm.name;
        firearm.preset = data;

        pickable.name=firearm.name;
        pickable.item = firearm;
        
    }
}
