using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject FirstButtonOfMainMenu;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(FirstButtonOfMainMenu);
    }
}
