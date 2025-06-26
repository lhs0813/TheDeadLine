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

            // Initialize and enable input controls
            _controls = new Controls();
            _controls.Enable();

            // Ensure the game starts unpaused
            FPSFrameworkCore.IsPaused = false;

            TabletController.onTabletShownAction += InputStop;
            TabletController.onTabletDisabledAction += InputStart;
        }

    string GetControlScheme()
    {
        // 마우스 이동 또는 클릭이 있었는지 감지
        if (Mouse.current != null)
        {
            if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f ||
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame)
            {
                return "Mouse";
            }
        }

        // 키보드 입력 감지
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return "Keyboard";
        }

        // 게임패드 버튼 또는 스틱 입력 감지
        if (Gamepad.current != null)
        {
            var g = Gamepad.current;
            if (g.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                g.dpad.ReadValue().sqrMagnitude > 0.1f ||
                g.buttonSouth.wasPressedThisFrame ||
                g.startButton.wasPressedThisFrame)
            {
                return "Gamepad";
            }
        }

        return _currentScheme; // 이전 스킴 유지
    }

        public GameObject firstButton; // 제일 위 버튼

        void ApplySchemeBehavior(string scheme)
        {
            if (scheme == "Gamepad")
            {
                Debug.Log("Gamepad Activate");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                // 가장 위 버튼 포커스
                if (firstButton != null)
                    EventSystem.current.SetSelectedGameObject(firstButton);
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // 마우스에서는 선택 포커스 해제 (안하면 키보드로 계속 hover됨)
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        /// <summary>
        /// Updates the Pause Menu. Listens for pause/unpause input.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (IsPaused)
            {
                string newScheme = GetControlScheme();
                if (newScheme != _currentScheme)
                {
                    Debug.Log(newScheme);
                    _currentScheme = newScheme;
                    ApplySchemeBehavior(_currentScheme);
                }               
            }


            //Menu Entering Control.
            if (_controls.UI.Pause.triggered)
            {
                if (_inputBlocked)
                    return;

                if (IsPaused)
                    Unpause();
                else
                    Pause();
            }

            if (IsPaused == false) CloseMenu();

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

        public void LoadScene(string sceneName)
        {
            LoadingScreen.LoadScene(sceneName);
        }

        void OnDisable()
        {
            TabletController.onTabletShownAction -= InputStop;
            TabletController.onTabletDisabledAction -= InputStart;
        }
    }
}
