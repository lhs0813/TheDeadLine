using Akila.FPSFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSequence : MonoBehaviour
{
    public CanvasGroup centerLogo;
    public CanvasGroup topLogo;
    public RectTransform creditsRoot;
    private Vector2 _initialCreditsPosition;
    // 크레딧 스크롤 속도 조절 변수
    public float scrollSpeedMultiplier = 1f; // 기본 스크롤 속도 배수
    public float scrollSpeed = 50f;
    public float creditsDuration = 20f; // 크레딧이 올라가는 시간

    private Vector2 _startPos;

    public Controls _controls;

    void Awake()
    {
        _initialCreditsPosition = creditsRoot.anchoredPosition;
    }

    void OnEnable()
    {
        centerLogo.alpha = 0f;
        topLogo.alpha = 0f;
        creditsRoot.anchoredPosition = _initialCreditsPosition;
        _startPos = _initialCreditsPosition;
        StartCoroutine(PlayCreditsSequence());
    }



    IEnumerator PlayCreditsSequence()
    {
        //메인메뉴 입력 비활성화
        FindAnyObjectByType<MainMenuInput>()._controls.Disable();
        //전용 입력 활성화
        _controls = new Controls();
        _controls.Disable();
        _controls.UI.Pause.Enable(); //ESC 조작만 사용가능.
        _controls.Player.Jump.Enable(); //가속
        _controls.Firearm.Fire.Enable(); //가속

        // ESC 키로 크레딧 시퀀스 스킵 매핑
        _controls.UI.Pause.performed += ctx =>
        {
            SkipCreditsSequence();
        };

        // 점프/발사 키 눌렀을 때 → 가속 ON
        _controls.Player.Jump.started += ctx => { scrollSpeedMultiplier = 2.5f; };
        _controls.Firearm.Fire.started += ctx => { scrollSpeedMultiplier = 2.5f; };

        // 점프/발사 키 뗐을 때 → 가속 OFF
        _controls.Player.Jump.canceled += ctx => { scrollSpeedMultiplier = 1f; };
        _controls.Firearm.Fire.canceled += ctx => { scrollSpeedMultiplier = 1f; };

        // 1. 중앙 큰 로고 페이드인
        yield return FadeCanvasGroup(centerLogo, 0, 1, 0.8f);
        yield return new WaitForSeconds(1f);

        // 2. 중앙 로고 페이드아웃
        yield return FadeCanvasGroup(centerLogo, 1, 0, 0.8f);

        // 3. 위쪽 로고 페이드인
        yield return FadeCanvasGroup(topLogo, 0, 1, 1f);

        // 4. 크레딧 올라가기 시작
        float elapsed = 0f;
        while (elapsed < creditsDuration)
        {
            float y = Mathf.Lerp(0, creditsDuration * scrollSpeed, elapsed / creditsDuration);
            creditsRoot.anchoredPosition = _startPos + Vector2.up * y;

            elapsed += Time.deltaTime * scrollSpeedMultiplier;
            yield return null;
        }


        // 5. 위쪽 로고 페이드아웃
        yield return FadeCanvasGroup(topLogo, 1, 0, 1f);

        // 6. 중앙 로고 다시 페이드인
        yield return FadeCanvasGroup(centerLogo, 0, 1, 1f);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = to;
    }

    void SkipCreditsSequence()
    {
        // 크레딧 UI 비활성화
        //centerLogo.gameObject.SetActive(false);
        //topLogo.gameObject.SetActive(false);
        creditsRoot.transform.root.gameObject.SetActive(false);


        _controls.Disable();
        _controls.Dispose();
        FindAnyObjectByType<MainMenuInput>()._controls.Enable();
    }
    void OnDisable()
    {
        StopAllCoroutines();
        if (_controls != null)
        {
            _controls.Disable();
            _controls.Dispose();
        }
    }

}
