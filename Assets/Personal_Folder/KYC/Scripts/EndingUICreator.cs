using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EscapeTrigger : MonoBehaviour
{
    [Header("화면 페이드용")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    [Header("페이드 후 보여줄 텍스트")]
    public TextMeshProUGUI messageText;
    public float textFadeDelay = 0.5f;
    public float textFadeDuration = 1f;

    [Header("결과 표시용 Number 텍스트")]
    public TextMeshProUGUI clearTimeNumberText;
    public TextMeshProUGUI bestClearTimeNumberText;

    [Header("나타날 버튼들")]
    public Button button1;
    public Button button2;
    public float button1Delay = 0.5f;
    public float button2Delay = 1f;

    private bool _isFading = false;
    private float clearTime;
    private float bestClearTime;

    private void Start()
    {
        // 메시지 & 버튼 초기 세팅
        if (messageText != null)
        {
            var tc = messageText.color;
            tc.a = 0f;
            messageText.color = tc;
        }
        button1?.gameObject.SetActive(false);
        button2?.gameObject.SetActive(false);

        // Number 텍스트도 처음엔 숨기기
        clearTimeNumberText?.transform.parent.gameObject.SetActive(false);
        bestClearTimeNumberText?.transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isFading || !other.CompareTag("Player")) return;

        // 1) 시간 계산 & 저장/로드
        clearTime = RecordManager.Instance.StopStoryTimer();
        bestClearTime = RecordManager.Instance.GetBestStoryTime();

        if (clearTime < 1200.0f)
            AchieveMent_Manager.Instance.CampaignFastClear();

         AchieveMent_Manager.Instance.CampaignClear();

        

        // 2) 포맷된 문자열 세팅
        if (clearTimeNumberText != null)
            clearTimeNumberText.text = FormatTime(clearTime);

        if (bestClearTimeNumberText != null)
        {
            bestClearTimeNumberText.text = bestClearTime == float.MaxValue
                ? "--:--.---"
                : FormatTime(bestClearTime);
        }

        // 3) 페이드 코루틴 시작
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        _isFading = true;

        // 화면 페이드
        float elapsed = 0f;
        Color screenCol = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            screenCol.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = screenCol;
            yield return null;
        }
        screenCol.a = 1f;
        fadeImage.color = screenCol;

        // 메시지 텍스트 페이드 인
        yield return new WaitForSecondsRealtime(textFadeDelay);
        if (messageText != null)
        {
            elapsed = 0f;
            var textCol = messageText.color;
            while (elapsed < textFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                textCol.a = Mathf.Clamp01(elapsed / textFadeDuration);
                messageText.color = textCol;
                yield return null;
            }
            textCol.a = 1f;
            messageText.color = textCol;
        }

        // 결과 텍스트(숫자) 활성화
        clearTimeNumberText?.transform.parent.gameObject.SetActive(true);
        bestClearTimeNumberText?.transform.parent.gameObject.SetActive(true);
        clearTimeNumberText?.gameObject.SetActive(true);
        bestClearTimeNumberText?.gameObject.SetActive(true);

        // 일시정지 & 커서 활성화
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 버튼들 활성화
        yield return new WaitForSecondsRealtime(button1Delay);
        button1?.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(button2Delay - button1Delay);
        button2?.gameObject.SetActive(true);
    }

    private string FormatTime(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        int ms = Mathf.FloorToInt((t * 1000f) % 1000f);
        return $"{m:00}:{s:00}.{ms:000}";
    }
}
