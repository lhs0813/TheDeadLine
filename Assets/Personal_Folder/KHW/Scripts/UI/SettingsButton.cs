using Akila.FPSFramework;
using Akila.FPSFramework.UI;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public void OpenSettingsMenu()
    {
        FindAnyObjectByType<MainMenuController>().DisableMenu();
        pauseMenu.SetUpForMainMenu(FindAnyObjectByType<MainMenuInput>()._controls);
    }

    [SerializeField] SettingsMenuController pauseMenu;
}
