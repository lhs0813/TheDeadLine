using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] GameObject FirstButtonOfMainMenu;

    void Start()
    {
        InputSchemeManager.OnSchemeChanged += ManageInputType;

        if (InputSchemeManager.CurrentScheme == InputSchemeManager.InputScheme.Gamepad)
        {
            StartCoroutine(WaitUntilButtonIsReadyAndSet());
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


    }

    private void ManageInputType(InputSchemeManager.InputScheme scheme)
    {
        if (scheme == InputSchemeManager.InputScheme.Gamepad)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StartCoroutine(WaitUntilButtonIsReadyAndSet());
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;            
        }
    }

    private IEnumerator WaitUntilButtonIsReadyAndSet()
    {
        yield return new WaitUntil(() => FirstButtonOfMainMenu != null && FirstButtonOfMainMenu.activeInHierarchy);
        EventSystem.current.SetSelectedGameObject(FirstButtonOfMainMenu);
    }

    void OnDestroy()
    {
        InputSchemeManager.OnSchemeChanged -= ManageInputType;
    }
}
