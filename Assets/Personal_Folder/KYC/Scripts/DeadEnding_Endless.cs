using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static Beautify.Universal.Beautify;

public class DeadEnding_Endless : MonoBehaviour
{
    [Header("화면 페이드용")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    [Header("페이드 후 보여줄 텍스트")]
    public TextMeshProUGUI messageText;
    public float textFadeDelay = 0.5f;
    public float textFadeDuration = 1f;

    [Header("결과 표시용 Score 텍스트")]
    public TextMeshProUGUI survivorRounds;
    public TextMeshProUGUI highSurvivorRounds;


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

        survivorRounds?.transform.parent.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        
    }

    public void Death()
    {
        // 현재 도달한 스테이지 계산
        int currentStage =  GamePlayManager.instance.currentMapIndex - 2;

        // 현재 스테이지 텍스트 표시
        if (survivorRounds != null)
            survivorRounds.text = "Station " + currentStage.ToString();

        // 최고 기록 저장 시도
        RecordManager.Instance.RecordInfiniteStage(currentStage);

        // 최고 기록 불러와서 표시
        int bestStage = RecordManager.Instance.LoadInfiniteStage();
        if (highSurvivorRounds != null)
            highSurvivorRounds.text = "Station " + bestStage.ToString();

        // 결과 텍스트 활성화
        survivorRounds.transform.parent.gameObject.SetActive(true);

        // 페이드 시퀀스 시작
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
        survivorRounds?.transform.parent.gameObject.SetActive(true);
        highSurvivorRounds?.transform.parent.gameObject.SetActive(true);



        // 일시정지 & 커서 활성화
        Time.timeScale = 0f;

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
