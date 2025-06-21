using Akila.FPSFramework;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Player_Hp_UI : MonoBehaviour
{
    
    TextMeshProUGUI[] HpUis;
    public Image hpBar;
    private float _maxHp = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HpUis = GetComponentsInChildren<TextMeshProUGUI>();
        //hpBar = transform.GetChild(4).GetComponent<Image>();
        Player_Manager.PlayerHpChange += HpUiUpdate;
        Player_Manager.PlayerMaxHpChange += MaxHpUiUpdate;
    }

    void HpUiUpdate(float hp)
    {
        HpUis[1].text = hp.ToString("F0"); // 1번은 NowHealth , 0번은 토탈 헬스~
        if (hp > 0)
        {
            hpBar.fillAmount = (float)(hp / _maxHp); 
            Debug.Log(hpBar);
            Debug.Log(hpBar.fillAmount);
        }
        else
            hpBar.fillAmount = 0;
    }

    void MaxHpUiUpdate(float maxHp)
    {
        HpUis[0].text =  "/ " + maxHp.ToString("F0");
        _maxHp = maxHp;
        
    }
}
