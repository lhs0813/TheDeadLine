using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSchemeManager : MonoBehaviour
{
    public enum InputScheme
    {
        Keyboard,
        Mouse,
        Gamepad
    }

    public static InputScheme CurrentScheme { get; private set; } = InputScheme.Mouse;
    public static event Action<InputScheme> OnSchemeChanged;

    private static InputSchemeManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        InputScheme newScheme = DetectScheme();

        if (newScheme != CurrentScheme)
        {
            CurrentScheme = newScheme;
            Debug.Log($"[InputSchemeManager] Switched to {CurrentScheme}");
            OnSchemeChanged?.Invoke(CurrentScheme);
        }
    }

    private InputScheme DetectScheme()
    {
        if (Mouse.current != null)
        {
            if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f ||
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame)
                return InputScheme.Mouse;
        }

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return InputScheme.Keyboard;
        }

        if (Gamepad.current != null)
        {
            var g = Gamepad.current;
            if (g.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                g.dpad.ReadValue().sqrMagnitude > 0.1f ||
                g.buttonSouth.wasPressedThisFrame ||
                g.startButton.wasPressedThisFrame)
                return InputScheme.Gamepad;
        }

        return CurrentScheme;
    }
}
