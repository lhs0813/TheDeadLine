using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject FirstButtonOfMainMenu;
    [SerializeField] GameObject MainMenuObj;
    [SerializeField] GameObject LeaderboardUI;
    [SerializeField] GameObject SNSUI;
    public GameObject creditsPanel;


    void Start()
    {
        InputSchemeManager.OnSchemeChanged += ManageInputType;

        Time.timeScale = 1f;

        if (InputSchemeManager.CurrentScheme == InputSchemeManager.InputScheme.Gamepad)
        {
            GoGamePadMod();
        }
        if (InputSchemeManager.CurrentScheme == InputSchemeManager.InputScheme.KeyboardAndMouse)
        {
            GoKMMod();
        }
    }

    private void ManageInputType(InputSchemeManager.InputScheme scheme)
    {
        if (scheme == InputSchemeManager.InputScheme.Gamepad)
        {
            GoGamePadMod();
        }
        if (scheme == InputSchemeManager.InputScheme.KeyboardAndMouse)
        {
            GoKMMod();
        }
    }

    private IEnumerator WaitUntilButtonIsReadyAndSet()
    {
        yield return new WaitUntil(() => FirstButtonOfMainMenu != null && FirstButtonOfMainMenu.activeInHierarchy);
        EventSystem.current.SetSelectedGameObject(FirstButtonOfMainMenu);
    }

    public void GoGamePadMod()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(WaitUntilButtonIsReadyAndSet());
        EventSystem.current.SetSelectedGameObject(FirstButtonOfMainMenu);
    }

    public void GoKMMod()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableMenu()
    {
        MainMenuObj.SetActive(false);
        LeaderboardUI.SetActive(false);
        SNSUI.SetActive(false);
    }

    public void EnableMenu()
    {
        MainMenuObj.SetActive(true);
        LeaderboardUI.SetActive(true);
        SNSUI.SetActive(true);
    }
    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        AchieveMent_Manager.Instance.ACHIEVEMENT_CREDIT();
    }
    void OnDisable()
    {
        InputSchemeManager.OnSchemeChanged -= ManageInputType;        
    }
}
