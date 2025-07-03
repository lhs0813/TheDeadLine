using System.Collections;
using UnityEngine;

public class TutorialTriggerUIController : MonoBehaviour
{
    public static TutorialTriggerUIController Instance;

    private GameObject _currentUI;
    private Coroutine _hideCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowUI(string triggerID)
    {
        // 이전 UI 있으면 끄기
        if (_currentUI != null)
            _currentUI.SetActive(false);

        // 새 UI 가져오기
        _currentUI = TutorialTriggerManager.Instance.GetUIByID(triggerID);
        if (_currentUI == null) return;

        _currentUI.SetActive(true);

        // 이전 코루틴 있으면 중단
        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        // 3초 뒤 자동 숨김
        _hideCoroutine = StartCoroutine(HideCurrentUIAfterDelay(2f));
    }

    private IEnumerator HideCurrentUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_currentUI != null)
        {
            _currentUI.SetActive(false);
            _currentUI = null;
        }
    }
}
