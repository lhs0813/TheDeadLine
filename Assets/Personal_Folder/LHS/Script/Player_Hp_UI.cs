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

        // 체력 텍스트 업데이트
        HpUis[1].text = hp.ToString("F0");

        // 체력 비율 계산
        float hpRatio = Mathf.Clamp01(hp / _maxHp);

        // 색상 보간 (흰색 → 빨간색)
        float tColor = 1f - hpRatio;
        Color color = Color.Lerp(Color.white, Color.red, tColor);
        HpUis[1].color = color;

        // 비네트용 보정된 보간값 (로그 스케일처럼 체감 되도록)
        float tVignette = 1f - Mathf.Pow(hpRatio, 0.3f); // 0.3f는 조절 가능

        // 체력바 및 비네트 효과 처리
        if (hp > 0)
        {
            hpBar.fillAmount = hpRatio;

            if (vignette != null)
            {
                vignette.intensity.value = tVignette * 1.2f;
            }
        }
        else
        {
            hpBar.fillAmount = 0;

            if (vignette != null)
            {
                vignette.intensity.value = 1f;
            }
        }
    }

    void MaxHpUiUpdate(float maxHp)
    {
        HpUis[0].text =  "/ " + maxHp.ToString("F0");
        _maxHp = maxHp;
        
    }
}
