using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseMenuController : MonoBehaviour
    {
        public static Action OnPauseMenuActivatedAction;
        public static Action OnPauseMenuDeactivatedAction;

        [Header("Fade Settings")]
        [Tooltip("Pause Menu 페이드 인/아웃에 걸리는 시간 (초)")]
        public float pauseMenuActivateTime = 0.4f;

        [Header("UI References")]
        [Tooltip("이 스크립트가 붙은 오브젝트에 있는 CanvasGroup")]
        public CanvasGroup canvasGroup;

        [Tooltip("설정창 컨트롤러 (null이어도 됨)")]
        public SettingsMenuController settingsMenu;

        private Controls _controls;
        private bool _isOpen;

        public bool IsOpened => _isOpen;

        public GameObject firstUIObj;

        void Start()
        {
            // CanvasGroup 초기 세팅
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            _isOpen = false;

            InputSchemeManager.OnSchemeChanged += ApplySchemeBehavior; // ✅
        }

        void ApplySchemeBehavior(InputSchemeManager.InputScheme scheme)
        {
            if (!IsOpened) return;

            Debug.Log(scheme);

            if (scheme == InputSchemeManager.InputScheme.Gamepad)
            {
                GoGamePadMod();

            }
            else
            {
                GoKeyboardMouseMod();
            }
        }

        private void GoGamePadMod()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //첫번째 버튼은 활성화.
            if (firstUIObj != null)
            {
                EventSystem.current.SetSelectedGameObject(null); // 먼저 선택 해제 (중복 방지)
                EventSystem.current.SetSelectedGameObject(firstUIObj);
            }
        }

        private void GoKeyboardMouseMod()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            EventSystem.current.SetSelectedGameObject(null); // 먼저 선택 해제 (중복 방지)
        }


        void OnEnable()
        {
            // Controls 인스턴스는 여기서만 new
            _controls = new Controls();
            _controls.Enable();

            // 메서드 그룹으로 구독
            _controls.UI.Pause.performed += OnPausePerformed;
        }

        void OnDisable()
        {
            _controls.UI.Pause.performed -= OnPausePerformed;
            _controls.Disable();

            FPSFrameworkCore.IsPaused = false;
        }

        private void OnPausePerformed(InputAction.CallbackContext ctx)
        {
            // 먼저 설정창이 열려 있으면 그것만 닫기
            if (_isOpen && settingsMenu != null && settingsMenu.IsOpen)
            {
                settingsMenu.HideSettingsMenu();
                return;
            }

            // 설정창이 없거나 닫혀 있으면 PauseMenu 토글
            if (_isOpen) DeactivatePauseMenu();
            else ActivatePauseMenu();
        }

        public void ActivatePauseMenu()
        {
            if (_isOpen || TabletController.isTabletActive) return;
            StartCoroutine(ShowPauseMenu());
        }

        public void DeactivatePauseMenu()
        {
            if (!_isOpen) return;
            StartCoroutine(HidePauseMenu());
        }

        private IEnumerator ShowPauseMenu()
        {
            // 1) 시간 멈춤
            FPSFrameworkCore.IsPaused = true;
            AudioListener.pause = true;
            Time.timeScale = 0f;
            _isOpen = true;

            // 2) CanvasGroup 활성화 상태로 전환
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // 3) 언스케일드 페이드 인
            float elapsed = 0f;
            while (elapsed < pauseMenuActivateTime)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / pauseMenuActivateTime);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            ApplySchemeBehavior(InputSchemeManager.CurrentScheme);

            OnPauseMenuActivatedAction?.Invoke();
        }

        private IEnumerator HidePauseMenu()
        {
            // 1) 언스케일드 페이드 아웃
            float start = canvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < pauseMenuActivateTime)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, 0f, elapsed / pauseMenuActivateTime);
                yield return null;
            }
            canvasGroup.alpha = 0f;

            // 2) 인터랙션 차단
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // 3) 시간 복구
            FPSFrameworkCore.IsPaused = false;
            Time.timeScale = 1f;
            _isOpen = false;
            AudioListener.pause = false;

            //조작 리셋
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            OnPauseMenuDeactivatedAction?.Invoke();
        }

        public void OpenSettingsMenu()
        {
            if (!_isOpen) return;

            // ① SettingsController에 Controls와 자기 자신 넘겨 주기
            settingsMenu.Setup(_controls, this);

            HidePauseUIOnly();

            // ② 그 다음에 실제 열기 호출
            settingsMenu.ShowSettingsMenu();
        }

        public void HidePauseUIOnly()
        {
            // 1) 페이드 아웃(언스케일드) 코루틴 대신 즉시
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // 2) _isOpen 은 그대로 true
            //    Time.timeScale 도 그대로 0 유지
        }

        public void ShowPauseUIOnly()
        {
            // 인터랙션 복원
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            // 언스케일드 페이드 인 코루틴 호출하거나 즉시
            canvasGroup.alpha = 1f;
        }

    }
    
    
    

}
