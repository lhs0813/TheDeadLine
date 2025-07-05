using System;
using TMPro;
using UnityEngine;

public class DangerStateUI : MonoBehaviour
{
    float remainingTime;
    bool isHidden;
    Animator _anim;

    public GameObject dangerNotice;
    Animator _dangerInfoAnim;

    private void Start()
    {
        isHidden = true;

        //GamePlayManager.instance.OnStationDepartAction += ShowUI;
        GamePlayManager.instance.OnStationArriveAction += InitializeState;
        GamePlayManager.instance.OnDangerAction += ShowUI;
        GamePlayManager.instance.OnPreDepartAction += HideUI;
        _anim = GetComponent<Animator>();
        _dangerInfoAnim = dangerNotice.GetComponent<Animator>();
    }

    private void InitializeState(float f)
    {
        isHidden = true;
    }

    private void HideUI()
    {
        if (!isHidden)
        {
            isHidden = true;
            _anim.SetTrigger("Off");
        }

    }

    private void ShowUI()
    {
        if (isHidden)
        {
            isHidden = false;
            _anim.SetTrigger("On");
            _dangerInfoAnim.SetTrigger("On");
        }

    }

    void Update()
    {

    }

    void OnDisable()
    {
        GamePlayManager.instance.OnStationArriveAction -= InitializeState;
    }
}
