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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HpUis = GetComponentsInChildren<TextMeshProUGUI>();
        //hpBar = transform.GetChild(4).GetComponent<Image>();
        Player_Manager.PlayerHpChange += HpUiUpdate;
    }

    void HpUiUpdate(float hp)
    {
        HpUis[1].text = hp.ToString("F0");
        if (hp > 0)
        {
            hpBar.fillAmount = (float)(hp / 100); 
            Debug.Log(hpBar);
            Debug.Log(hpBar.fillAmount);
        }
        else
            hpBar.fillAmount = 0;
    }
}
