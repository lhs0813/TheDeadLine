using System;
using TMPro;
using UnityEngine;

public class GamePlayManagementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void UpdateGamePlayUI(bool isOnCombat, float currentTime, float nextTime)
    {

        if (!isOnCombat)
        {
            _anim.SetTrigger("Off");
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
