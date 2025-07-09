using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemainingTimeForInvestigationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTimeText;

    public Image departureBack;
    public Image trainImage;
    public TextMeshProUGUI departureText;
    public TextMeshProUGUI departureRemainText;



    private bool _hasColorChanged = false;

    float remainingTime;
    bool isHidden;
    Animator _anim;

    private void Start()
    {
        isHidden = true;

        GamePlayManager.instance.OnStationArriveAction += ShowUI;
        GamePlayManager.instance.OnPreDepartAction += HideUI;
        // GamePlayManager.instance.OnDangerAction += HideUI;
        // GamePlayManager.instance.OnPreDepartAction += HideUI;
        _anim = GetComponent<Animator>();
    }

    private void ColorChange(Color color)
    {
        departureBack.color = color;
        trainImage.color = color;
        departureText.color = color;
        departureRemainText.color = color;
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

        Color greenWithAlpha = new Color(0f, 1f, 0f, 0.666f);
        ColorChange(greenWithAlpha);

        _hasColorChanged = false;

        _anim.SetTrigger("On");
    }

    public void UpdateRemainingTime(bool isOnCombat, float currentTime, float nextTime)
    {
        float remaining = Mathf.Max(0f, nextTime - currentTime);

        // 1) TimeSpan 사용
        TimeSpan t = TimeSpan.FromSeconds(remaining);
        // t.Minutes, t.Seconds, t.Milliseconds

        remainingTimeText.text =
            $"{t.Minutes:00}:{t.Seconds:00}:{t.Milliseconds:000}";
        //Debug.Log($"전투중 : {isOnCombat}, 남은시간 : {t}");
    }

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

            if (remainingTime < 60f && !_hasColorChanged)
            {
                Color orangeWithAlpha = new Color(1f, 0.35f, 0f, 0.666f);
                ColorChange(orangeWithAlpha);

                _hasColorChanged = true;
            }
        }
        else if (!isHidden && remainingTime <= 0f)
        {
            HideUI();
        }



    }

    void OnDisable()
    {
        GamePlayManager.instance.OnStationArriveAction -= ShowUI;
        GamePlayManager.instance.OnPreDepartAction -= HideUI;
    }
}
