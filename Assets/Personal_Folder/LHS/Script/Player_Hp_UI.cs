using Akila.FPSFramework;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // ✅ 중요!

public class Player_Hp_UI : MonoBehaviour
{
    
    TextMeshProUGUI[] HpUis;
    public Image hpBar;
    private float _maxHp = 100;

    public GameObject hpIcon;
    private Animator hpIconAnim;

    public Volume volume;
    Vignette vignette;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HpUis = GetComponentsInChildren<TextMeshProUGUI>();
        //hpBar = transform.GetChild(4).GetComponent<Image>();
        Player_Manager.PlayerHpChange += HpUiUpdate;
        Player_Manager.PlayerMaxHpChange += MaxHpUiUpdate;

        volume.profile.TryGet(out vignette);

        hpIconAnim = hpIcon.GetComponent<Animator>();
    }

    /*public void ApplyDamageEffect()
    {
        if (vignette != null)
        {
            vignette.intensity.value = 0.5f;
            vignette.color.value = Color.red;
        }
    }*/

    void HpUiUpdate(float hp)
    {
        hpIconAnim.SetTrigger("On");
        HpUis[1].text = hp.ToString("F0");

        if (hp > 0)
        {
            hpBar.fillAmount = (float)(hp / _maxHp);

            // 🔴 Vignette 효과 동기화
            if (vignette != null)
            {
                vignette.intensity.value = 1f - (hp / _maxHp);
            }
        }
        else
        {
            hpBar.fillAmount = 0;

            if (vignette != null)
            {
                vignette.intensity.value = 1f; // 체력 0일 때 최대 강도
            }
        }
    }

    void MaxHpUiUpdate(float maxHp)
    {
        HpUis[0].text =  "/ " + maxHp.ToString("F0");
        _maxHp = maxHp;
        
    }
}
