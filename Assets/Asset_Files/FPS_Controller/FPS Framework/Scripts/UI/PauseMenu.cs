using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Akila.FPSFramework.UI
{
    /// <summary>
    /// Represents a Pause Menu in the FPS Framework. 
    /// Handles pausing and unpausing the game and updating the UI accordingly.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/UI/Pause Menu")]
    public class PauseMenu : Menu
    {
        /// <summary>
        /// Input controls for the pause menu.
        /// </summary>
        private Controls _controls;

        /// <summary>
        /// Indicates whether the game is currently paused.
        /// </summary>
        public bool IsPaused => FPSFrameworkCore.IsPaused;

        private bool _inputBlocked = false;

        private string _currentScheme = "";

        /// <summary>
        /// Initializes the Pause Menu.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _controls = new Controls();
            _controls.Enable();

            FPSFrameworkCore.IsPaused = false;

            TabletController.onTabletShownAction += InputStop;
            TabletController.onTabletDisabledAction += InputStart;

            InputSchemeManager.OnSchemeChanged += ApplySchemeBehavior; // ✅
        }

        public GameObject firstButton; // 제일 위 버튼

        void ApplySchemeBehavior(InputSchemeManager.InputScheme scheme)
        {
            if (!IsPaused) return;

            Debug.Log(scheme);

            if (scheme == InputSchemeManager.InputScheme.Gamepad)
            {
                GoGamePadMod();

            }
            else
            {
                GoKeyboardMouseMod();
                // Cursor.visible = true;
                // Cursor.lockState = CursorLockMode.None;

                // EventSystem.current.SetSelectedGameObject(null);
            }
        }


        /// <summary>
        /// Updates the Pause Menu. Listens for pause/unpause input.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (_controls.UI.Pause.triggered)
            {
                if (_inputBlocked)
                    return;

                if (IsPaused)
                    Unpause();
                else
                    Pause();
            }

            if (!IsPaused)
                CloseMenu();
        }


        private void InputStop()
        { 
            _inputBlocked = true;
            _controls.Disable();
        }

        private void InputStart()
        {
            _inputBlocked = false;
            _controls.Enable();
        }



        /// <summary>
        /// Pauses the game and opens the pause menu.
        /// </summary>
        public void Pause()
        {
            // Update game state to paused
            FPSFrameworkCore.IsPaused = true;

            ApplySchemeBehavior(InputSchemeManager.CurrentScheme);

            OpenMenu();
        }

        /// <summary>
        /// Unpauses the game and closes the pause menu.
        /// </summary>
        public void Unpause()
        {
            // Close the pause menu UI
            if (IsOpen)
            {
                // Update game state to unpaused
                FPSFrameworkCore.IsPaused = false;

                // Lock the cursor and hide it
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void GoGamePadMod()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //첫번째 버튼은 활성화.
            if (firstButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null); // 먼저 선택 해제 (중복 방지)
                EventSystem.current.SetSelectedGameObject(firstButton);

                // 시각적 하이라이트 강제 적용
                var selectable = firstButton.GetComponent<Selectable>();
                if (selectable != null)
                {
                    selectable.OnSelect(null); // null은 BaseEventData
                }
            }
        }

        private void GoKeyboardMouseMod()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void LoadScene(string sceneName)
        {
            LoadingScreen.LoadScene(sceneName);
        }

        void OnDisable()
        {
            TabletController.onTabletShownAction -= InputStop;
            TabletController.onTabletDisabledAction -= InputStart;
            InputSchemeManager.OnSchemeChanged -= ApplySchemeBehavior; // ✅
        }

    }
}
