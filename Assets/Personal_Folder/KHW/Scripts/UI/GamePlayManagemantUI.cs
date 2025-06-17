using System;
using TMPro;
using UnityEngine;

public class GamePlayManagementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    Animator _anim;

    private void Start()
    {
        GamePlayManager.instance.OnStationArriveAction += ShowCombatUI;
        GamePlayManager.instance.OnStationDepartAction += ShowWaitingUI;
        _anim = GetComponent<Animator>();
    }

    private void ShowWaitingUI()
    {
        //기차 내부 대기를 시작할 때 보여줄 UI 코드.
        _anim.SetTrigger("Off");
        //_anim.ResetTrigger("Off");
    }

    private void ShowCombatUI()
    {
            Debug.Log($"출발 남은시간 UI 표출. {gameObject.name}");
            _anim.SetTrigger("On");
            //_anim.ResetTrigger("On");            
    }

    public void UpdateRemainingTimeUI(bool isOnCombat, float currentTime, float nextTime)
    {

        if (!isOnCombat)
        {

            remainingTimeText.text = "";
            return;
        }


        float remaining = Mathf.Max(0f, nextTime - currentTime);

        // 1) TimeSpan 사용
        TimeSpan t = TimeSpan.FromSeconds(remaining);
        // t.Minutes, t.Seconds, t.Milliseconds

        remainingTimeText.text =
            $"{t.Minutes:00}:{t.Seconds:00}:{t.Milliseconds:000}";
        //Debug.Log($"전투중 : {isOnCombat}, 남은시간 : {t}");
    }

    
}
