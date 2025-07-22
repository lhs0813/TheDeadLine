using System.Collections.Generic;
using UnityEngine;

public class HandMat_Holder : MonoBehaviour
{
    public static HandMat_Holder instance;

    public static Texture currentTexture;
    public static List<Texture> textures;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        // HandTextures 폴더 안의 모든 Texture2D를 불러옴
        Texture2D[] loadedTextures = Resources.LoadAll<Texture2D>("HandTextures");
        textures = new List<Texture>(loadedTextures);
        currentTexture = textures[2]; // 기본 텍스쳐는 2번
    }

    public void SetTexture(int index)
    {
        currentTexture = textures[index];
    }
}
