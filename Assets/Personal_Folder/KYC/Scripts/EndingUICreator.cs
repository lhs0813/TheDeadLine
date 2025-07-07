using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EscapeTrigger : MonoBehaviour
{
    // Inspector에서 화면 꽉 찬 Image 할당
    public Image fadeImage;
    // 페이드 지속시간 (초)
    public float fadeDuration = 2f;

    // 한 번만 페이드하도록 플래그
    private bool _isFading = false;

    // 플레이어가 트리거에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (!_isFading && other.CompareTag("Player"))
        {
            StartCoroutine(FadeToWhite());
        }
    }

    // 알파를 0→1 로 올리는 코루틴
    private IEnumerator FadeToWhite()
    {
        _isFading = true;
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // 완전히 하얗게
        c.a = 1f;
        fadeImage.color = c;

        // TODO: 화면 전환이나 다음 로직 호출
    }
}
