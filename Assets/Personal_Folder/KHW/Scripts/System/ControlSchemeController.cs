using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class InputSchemeManager : MonoBehaviour
{
    public enum InputScheme
    {
        Keyboard,
        Mouse,
        Gamepad
    }

    public enum GamepadType
    {
        None,
        Xbox,
        PlayStation,
        Other
    }

    public static InputScheme CurrentScheme { get; private set; } = InputScheme.Mouse;
    public static GamepadType CurrentGamepadType { get; private set; } = GamepadType.None;

    public static event Action<InputScheme> OnSchemeChanged;
    public static event Action<GamepadType> OnGamepadTypeChanged;

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
        // 1) 스킴 감지
        var newScheme = DetectScheme();
        if (newScheme != CurrentScheme)
        {
            CurrentScheme = newScheme;
            OnSchemeChanged?.Invoke(CurrentScheme);
        }

        // 2) 만약 Gamepad 모드라면, 패드 종류도 감지
        if (CurrentScheme == InputScheme.Gamepad)
        {
            var newPadType = DetectGamepadType();
            if (newPadType != CurrentGamepadType)
            {
                CurrentGamepadType = newPadType;
                OnGamepadTypeChanged?.Invoke(CurrentGamepadType);
            }
        }
        else if (CurrentGamepadType != GamepadType.None)
        {
            // Gamepad 모드 벗어나면 None 리셋
            CurrentGamepadType = GamepadType.None;
            OnGamepadTypeChanged?.Invoke(CurrentGamepadType);
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
            return InputScheme.Keyboard;

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

    private GamepadType DetectGamepadType()
    {
        var g = Gamepad.current;
        if (g == null)
            return GamepadType.None;

        // DualShock / DualSense 클래스 체크
        if (g is DualShockGamepad || g is DualSenseGamepadHID)
            return GamepadType.PlayStation;

        // 그 외엔 대체로 XInput 기반 Xbox 패드
        var desc = g.description;
        if (desc.interfaceName != null && desc.interfaceName.Contains("XInput"))
            return GamepadType.Xbox;
        if (desc.product != null && desc.product.ToLower().Contains("xbox"))
            return GamepadType.Xbox;

        // 알 수 없는 패드
        return GamepadType.Other;
    }
}
