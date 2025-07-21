using System.Collections.Generic;
using UnityEngine;

public class ArmChange : MonoBehaviour
{
    public Material targetMaterial;        // 메테리얼 직접 할당
    public List<Texture> textures;
    public int textureIndex = 0;

    public void ChangeArm()
    {
        if (targetMaterial != null && textures != null && textureIndex < textures.Count)
        {
            targetMaterial.SetTexture("_BaseMap", textures[textureIndex]);
        }
    }
}