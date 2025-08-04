using System.Collections;
using Akila.FPSFramework;
using Akila.FPSFramework.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ReturnConfirmMenuController : MonoBehaviour
{
        [Header("Fade Settings")]
        [Tooltip("Settings Menu 페이드 인/아웃에 걸리는 시간 (초)")]
        public float settingsMenuActivateTime = 0.4f;

        private Controls _controls;
        private CanvasGroup _canvasGroup;
        private bool _isOpen;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        // 외부에서 PauseMenuController가 넘겨줌
        private PauseMenuController _pauseMenuController;

        public bool IsOpen => _isOpen;

        [Header("Manage Setting Executions")]
        public GameObject firstUIObj;

        void Start()
        {
            // CanvasGroup 초기 세팅
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _isOpen = false;

            
        }

        /// <summary>
        /// PauseMenu에서 인스턴스와 자기 참조를 넘겨 호출
        /// </summary>
        public void Setup(Controls sharedControls, PauseMenuController pauseBy)
        {
            _controls = sharedControls;
            _pauseMenuController = pauseBy;
            InputSchemeManager.OnSchemeChanged += SetInputMode;

            // Pause 액션을 Settings 닫기로 바인딩
            _controls.UI.Pause.performed += OnPausePerformed;
            _controls.UI.ReturnToPreviousMenu.performed += OnPausePerformed;
        }

        private void SetInputMode(InputSchemeManager.InputScheme scheme)
        {
            if (scheme == InputSchemeManager.InputScheme.Gamepad)
            {
                EventSystem.current.SetSelectedGameObject(firstUIObj);
            }
            else
            {
                
            }
        }


        void OnDisable()
        {
            if (_controls != null)
            {
                _controls.UI.Pause.performed -= OnPausePerformed;
                _controls.UI.ReturnToPreviousMenu.performed -= OnPausePerformed;
            }

            InputSchemeManager.OnSchemeChanged -= SetInputMode;
        }

        private void OnPausePerformed(InputAction.CallbackContext ctx)
        {
            // 설정창 열려 있을 때만 닫는다
            if (_isOpen)
                HideReturnConfirmMenu();
        }

        /// <summary>
        /// Pause 메뉴가 열려 있을 때만 호출
        /// </summary>
        public void ShowReturnConfirmMenu()
        {
            if (!_pauseMenuController.IsOpened || _isOpen) return;

            StartCoroutine(ShowReturnConfirmMenuCoroutine());
        }

        /// <summary>
        /// 페이드 인 후 _isOpen=true
        /// </summary>
        private IEnumerator ShowReturnConfirmMenuCoroutine()
        {
            _isOpen = true;

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;


            float elapsed = 0f;
            while (elapsed < settingsMenuActivateTime)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / settingsMenuActivateTime);
                yield return null;
            }
            _canvasGroup.alpha = 1f;


            SetInputMode(InputSchemeManager.CurrentScheme);
        }

        /// <summary>
        /// 설정창 닫기
        /// </summary>
        public void HideReturnConfirmMenu()
        {
            if (!_isOpen) return;
            StartCoroutine(HideReturnConfirmCoroutine());
        }

        private IEnumerator HideReturnConfirmCoroutine()
        {

            
            float start = _canvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < settingsMenuActivateTime)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(start, 0f, elapsed / settingsMenuActivateTime);
                yield return null;
            }
            _canvasGroup.alpha = 0f;

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _isOpen = false;

            if (_pauseMenuController != null)
            {
                _pauseMenuController.ShowPauseUIOnly();
            } //이전의 pauseMenu 없으면 개방 없음.
            if (FindAnyObjectByType<MainMenuController>() is MainMenuController m)
            {
                m.EnableMenu();
            }

        }

        


}
