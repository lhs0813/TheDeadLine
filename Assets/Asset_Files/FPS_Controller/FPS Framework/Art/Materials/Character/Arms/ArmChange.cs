using Akila.FPSFramework;
using System.Collections.Generic;
using UnityEngine;

public class ArmChange : MonoBehaviour
{
    public int textureIndex = 0;


    public void ChangeArm()
    {
        HandMat_Holder.instance.SetTexture(textureIndex);

        List<Firearm> firearms = new List<Firearm>(FindObjectsByType<Firearm>(FindObjectsInactive.Include, FindObjectsSortMode.None));

        foreach (var v in firearms)
            Debug.Log(v.gameObject.name);

        foreach (var firearm in firearms)
        {
            firearm.SetTexture();
        }

        /* if (targetMaterial != null && textures != null && textureIndex < textures.Count)
         {
             targetMaterial.SetTexture("_BaseMap", textures[textureIndex]);
         }*/
    }
}