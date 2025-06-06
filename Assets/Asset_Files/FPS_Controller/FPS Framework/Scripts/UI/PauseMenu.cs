using UnityEngine;

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

            RotateOnTrigger.onLapTop += InputStop;
            RotateOnTrigger.offLapTop += InputStart;
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

            if(IsPaused == false) CloseMenu();
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

            // Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Open the pause menu UI
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
    }
}
