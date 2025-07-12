using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject FirstButtonOfMainMenu;
    [SerializeField] GameObject MainMenuObj;
    [SerializeField] GameObject LeaderboardUI;

    void Start()
    {
        InputSchemeManager.OnSchemeChanged += ManageInputType;

        Time.timeScale = 1f;
    }

    private void ManageInputType(InputSchemeManager.InputScheme scheme)
    {
        if (scheme == InputSchemeManager.InputScheme.Gamepad)
        {
            GoGamePadMod();
        }
    }

    public void GoGamePadMod()
    {
        EventSystem.current.SetSelectedGameObject(FirstButtonOfMainMenu);
    }

    public void DisableMenu()
    {
        MainMenuObj.SetActive(false);
        LeaderboardUI.SetActive(false);
    }

    public void EnableMenu()
    {
        MainMenuObj.SetActive(true);
        LeaderboardUI.SetActive(true);
    }

    void OnDisable()
    {
        InputSchemeManager.OnSchemeChanged -= ManageInputType;        
    }
}
