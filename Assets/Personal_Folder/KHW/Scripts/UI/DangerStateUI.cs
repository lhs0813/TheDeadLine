using System;
using TMPro;
using UnityEngine;

public class DangerStateUI : MonoBehaviour
{
    float remainingTime;
    bool isHidden;
    Animator _anim;

    private void Start()
    {
        isHidden = true;

        //GamePlayManager.instance.OnStationDepartAction += ShowUI;
        //GamePlayManager.instance.OnStationArriveAction += ShowUI;
        GamePlayManager.instance.OnDangerAction += ShowUI;
        GamePlayManager.instance.OnPreDepartAction += HideUI;
        _anim = GetComponent<Animator>();
    }

    private void HideUI()
    {
        isHidden = true;
        _anim.SetTrigger("Off");
    }

    private void ShowUI()
    {
        isHidden = false;
        _anim.SetTrigger("On");
    }

    void Update()
    {

    }

    void OnDisable()
    {
        GamePlayManager.instance.OnDangerAction -= ShowUI;
        GamePlayManager.instance.OnPreDepartAction -= HideUI;
    }
}
