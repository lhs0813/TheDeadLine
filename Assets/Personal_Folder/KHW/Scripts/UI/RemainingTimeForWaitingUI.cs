using System;
using TMPro;
using UnityEngine;

public class RemainingTimeForWaitingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    float remainingTime;
    bool isHidden;
    Animator _anim;


    private void Start()
    {
        isHidden = true;

        GamePlayManager.instance.OnStationDepartAction += ShowUI;
        GamePlayManager.instance.OnTrainAccelerationAction += ControlAcceleration;
        //GamePlayManager.instance.OnStationArriveAction += ShowUI;
        // GamePlayManager.instance.OnDangerAction += HideUI;
        // GamePlayManager.instance.OnPreDepartAction += HideUI;
        _anim = GetComponent<Animator>();
    }

    private void HideUI()
    {
        isHidden = true;
        _anim.SetTrigger("Off");
    }

    private void ShowUI(float remainingTime)
    {
        this.remainingTime = remainingTime;
        isHidden = false;
        _anim.SetTrigger("On");
    }

    // public void UpdateRemainingTime(bool isOnCombat, float currentTime, float nextTime)
    // {
    //     float remaining = Mathf.Max(0f, nextTime - currentTime);

    //     // 1) TimeSpan 사용
    //     TimeSpan t = TimeSpan.FromSeconds(remaining);
    //     // t.Minutes, t.Seconds, t.Milliseconds

    //     remainingTimeText.text =
    //         $"{t.Minutes:00}:{t.Seconds:00}:{t.Milliseconds:000}";
    //     //Debug.Log($"전투중 : {isOnCombat}, 남은시간 : {t}");
    // }

    void Update()
    {
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;

            float remaining = Mathf.Max(0f, remainingTime);

            // 1) TimeSpan 사용
            TimeSpan t = TimeSpan.FromSeconds(remaining);
            // t.Minutes, t.Seconds, t.Milliseconds

            remainingTimeText.text =
            $"{t.Minutes:00}:{t.Seconds:00}:{t.Milliseconds:000}";
            //Debug.Log($"전투중 : {isOnCombat}, 남은시간 : {t}");
        }
        else if (!isHidden && remainingTime <= 0f)
        {
            HideUI();

        }
    }

    public void ControlAcceleration(float accelerationRemainingTime)
    {
        this.remainingTime = accelerationRemainingTime;
        _anim.SetTrigger("Accelerate");
    }

    void OnDisable()
    {
        GamePlayManager.instance.OnStationDepartAction += ShowUI;
    }
}
