using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Ad_Board : MonoBehaviour
{
    public Image weaponImage;                          // 화면에 표시될 이미지
    public TextMeshProUGUI weaponName;                 // 이미지 이름 텍스트
    public List<Sprite> weaponSpriteList;              // 미리 할당된 무기 이미지 리스트

    void Start()
    {
        if (weaponSpriteList == null || weaponSpriteList.Count == 0)
        {
            Debug.LogWarning("weaponSpriteList가 비어있습니다.");
            return;
        }

        int randomIndex = Random.Range(0, weaponSpriteList.Count);
        Sprite selectedSprite = weaponSpriteList[randomIndex];

        weaponImage.sprite = selectedSprite;
        weaponName.text = selectedSprite.name;
    }
}
