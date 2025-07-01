using Akila.FPSFramework;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsControl : MonoBehaviour
{
    public List<GameObject> list = new();
    //public List<GameObject> list2 = new();

    //   public Material material;
    public FirearmPreset data;




    void OnEnable()
    {
        ActiveMeshRender();
        //InstPickables();
        // EraseMeshCollider();
    }

    void Update()
    {
        int start;
        for (int i = (int)KeyCode.Alpha0; i < (int)KeyCode.Alpha9; ++i)
        {
            if (Input.GetKeyDown((KeyCode)i))            
                Debug.Log(i - ((int)KeyCode.Alpha0));            
        }
    }

    void ActiveMeshRender()
    {
        var v = list[0].GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < v.Length; i++)
            v[i].enabled=true;



    }



        void WeaponMaterial()
        {

            //for (int i = 0; i < list.Count; i++)
            //{
            //    var v = list[i].GetComponent<Pickable>();
            //    if (v)
            //    { 
            //        v.GetComponentInChildren<Renderer>().materials. = material;

            //    }
            //}




        }

    
        void InstPickables()
        {
            List<GameObject> temp = new();
            for (int i = 0; i < list.Count; i++)
            {
                var v = list[i].GetComponent<Pickable>();
                if (v)
                {
                    temp.Add(list[i]);
                }
            }

            for (int i = 0; i < temp.Count; i++)
            {
                Instantiate(temp[i], transform.position + Vector3.right * 3 * i + Vector3.up * 2, transform.rotation);
            }
        }


        void EraseMeshCollider()
        {
            var v = list[0].GetComponentsInChildren<MeshCollider>(true);
            for (int i = 0; i < v.Length; i++)
                Destroy(v[i]);

        }



        void WeaponSetting()
        {
            Firearm firearm = null;
            Pickable pickable = null;
            Projectile projectile = null;
            for (int i = 0; i < list.Count; i++)
            {
                var v = list[i].GetComponent<Firearm>();
                if (v)
                    firearm = v;

                var v1 = list[i].GetComponent<Pickable>();
                if (v1)
                    pickable = v1;

                var v2 = list[i].GetComponent<Projectile>();
                if (v2)
                    projectile = v2;
            }

            data.replacement = pickable;
            data.projectile = projectile;

            firearm.Name = firearm.name;
            firearm.preset = data;

            pickable.Name = firearm.name;
            pickable.item = firearm;

        }
    
}

/*
   System.IO.DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/Personal_Folder/KSH");
        foreach (FileInfo file in di.GetFiles("*.prefab",SearchOption.AllDirectories))
        {
            file.;
            Debug.Log("ÆÄÀÏ¸í : " + file.Name);
        }


       // Asset_Files FPS_Controller

 */