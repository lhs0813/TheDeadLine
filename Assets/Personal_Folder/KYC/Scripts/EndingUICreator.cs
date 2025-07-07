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

    [Header("나타날 버튼들")]
    public Button button1;
    public Button button2;
    public float button1Delay = 0.5f; // 텍스트 페이드 끝나고 이만큼 기다린 뒤 첫 버튼
    public float button2Delay = 1f;   // 텍스트 페이드 끝나고 이만큼 기다린 뒤 두 번째 버튼

    private bool _isFading = false;
    public float ClearTime;

    private void Start()
    {
        // 시작할 때 텍스트·버튼들 투명/비활성화
        if (messageText != null)
        {
            var tc = messageText.color;
            tc.a = 0f;
            messageText.color = tc;
        }
        if (button1 != null) button1.gameObject.SetActive(false);
        if (button2 != null) button2.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isFading && other.CompareTag("Player"))
        {
            StartCoroutine(FadeSequence());
            RecordManager.Instance.StopStoryTimer();
            ClearTime =  RecordManager.Instance.LoadStoryTime();
        }
    }

    private IEnumerator FadeSequence()
    {
        _isFading = true;

        // 1) 화면 페이드
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

        // 2) 텍스트 페이드 인
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

        // 3) 게임 일시정지 & 커서 활성화
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 4) 버튼들 시간차로 활성화
        if (button1 != null)
        {
            yield return new WaitForSecondsRealtime(button1Delay);
            button1.gameObject.SetActive(true);
        }
        if (button2 != null)
        {
            yield return new WaitForSecondsRealtime(button2Delay - button1Delay);
            button2.gameObject.SetActive(true);
        }
        
        // TODO: 이후 로직…
    }
}
