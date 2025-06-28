using Akila.FPSFramework;
using UnityEngine;

public class MainMenuInput : MonoBehaviour
{
    public Controls _controls;

    void Start()
    {
        _controls = new Controls();
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }

}
